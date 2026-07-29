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

namespace INPS.Pensioni.LiquidazioneAgo
{
    public class GestionePrelievo
    {
        #region public members
        public static void PrelevaDomanda(RichiestaPrelievo richiesta, out RispostaPrelievo risposta, out string messaggioVideo)
        {
            risposta = null;
            messaggioVideo = "";
            Data.GAIN AreaPrelievo = null;
            Data.GAIN_New AreaPrelievoNew = null;
            bool inseguiPensione = true;
            DateTime dataSistema = Utility.DataSistemaAgo;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            bool isNuovoTracciato = false;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoData = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioNuovoTracciato", out controlloDinamicoData);
            DateTime dataInizioNuovoTracciato = Utility.DataFromString(controlloDinamicoData.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();
            // Se è una Ric o TRF e la data sistema è compresa tra i due controlli dinamici(DataInizioInterregno e DataFineInterregno)
            // oppure se la data sistema è maggiore uguale al 01/12/2023 viene eseguito il nuovo tracciato
            if ((richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                   && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno)) ||
                Utility.DataSuccessivaA(dataSistema, dataInizioNuovoTracciato))
                isNuovoTracciato = true;

            while (inseguiPensione)
            {
                if (!isNuovoTracciato)
                {
                    ValorizzaAreaPrelievo(richiesta, out AreaPrelievo, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                        return;
                    Guid guid = Guid.NewGuid();
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Request, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.IN, richiesta.NumDomanda, guid);
                    EseguiPrelievo(AreaPrelievo);

                    ControllaEsitoPrelievo(AreaPrelievo, out messaggioVideo);

                    if (AreaPrelievo.HasError)
                        GestioneLogSoap.SalvaLogSoap(messaggioVideo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);
                    else
                        GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Response, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);

                    if (!string.IsNullOrEmpty(AreaPrelievo.MessaggioDaLoggare))
                    {
                        long numeroDomanda = 0;
                        long.TryParse(richiesta != null ? richiesta.NumDomanda : null, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaPrelievo.MessaggioDaLoggare, null, null);
                    }
                    if (!String.IsNullOrEmpty(messaggioVideo))
                        return;

                    if (AreaPrelievo.Response != null && AreaPrelievo.Response.Pagamento != null)
                    {
                        if (AreaPrelievo.Response.Pagamento.T_GP1AM01_V == "5")
                        {
                            richiesta.Categoria = 0;
                            richiesta.Sede = 0;
                            richiesta.Certificato = 0;

                            if (AreaPrelievo.Response.Intestazione != null)
                            {
                                short categoria = 0;
                                short.TryParse(AreaPrelievo.Response.Intestazione.T_GP1AM07, out categoria);
                                richiesta.Categoria = categoria;
                                richiesta.Sede = AreaPrelievo.Response.Intestazione.T_GP1AM08;
                                richiesta.Certificato = AreaPrelievo.Response.Intestazione.T_GP1AM09;
                            }

                            if (richiesta.Categoria == 0 || richiesta.Sede == 0 || richiesta.Certificato == 0)
                            {
                                messaggioVideo = "ATTENZIONE! Non è individuabile la pensione vigente per la domanda.";
                                return;
                            }
                        }
                        else
                            inseguiPensione = false;
                    }
                    else
                        inseguiPensione = false;
                }
                else
                {
                    ValorizzaAreaPrelievoNew(richiesta, out AreaPrelievoNew, out messaggioVideo);
                    if (!String.IsNullOrEmpty(messaggioVideo))
                        return;
                    Guid guid = Guid.NewGuid();
                    GestioneLogSoap.SalvaLogSoap(AreaPrelievoNew.Request, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.IN, richiesta.NumDomanda, guid);
                    EseguiPrelievoNuovoTracciato(AreaPrelievoNew);

                    ControllaEsitoPrelievoNuovoTracciato(AreaPrelievoNew, out messaggioVideo);

                    if (AreaPrelievoNew.HasError)
                        GestioneLogSoap.SalvaLogSoap(messaggioVideo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);
                    else
                        GestioneLogSoap.SalvaLogSoap(AreaPrelievoNew.ResponseNew, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.OUT, richiesta.NumDomanda, guid);

                    if (!string.IsNullOrEmpty(AreaPrelievoNew.MessaggioDaLoggare))
                    {
                        long numeroDomanda = 0;
                        long.TryParse(richiesta != null ? richiesta.NumDomanda : null, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, AreaPrelievoNew.MessaggioDaLoggare, null, null);
                    }
                    if (!String.IsNullOrEmpty(messaggioVideo))
                        return;

                    if (AreaPrelievoNew.ResponseNew != null && AreaPrelievoNew.ResponseNew.Pagamento != null)
                    {
                        if (AreaPrelievoNew.ResponseNew.Pagamento.T_GP1AM01_V == "5")
                        {
                            richiesta.Categoria = 0;
                            richiesta.Sede = 0;
                            richiesta.Certificato = 0;

                            if (AreaPrelievoNew.ResponseNew.Intestazione != null)
                            {
                                short categoria = 0;
                                short.TryParse(AreaPrelievoNew.ResponseNew.Intestazione.T_GP1AM07, out categoria);
                                richiesta.Categoria = categoria;
                                richiesta.Sede = AreaPrelievoNew.ResponseNew.Intestazione.T_GP1AM08;
                                richiesta.Certificato = AreaPrelievoNew.ResponseNew.Intestazione.T_GP1AM09;
                            }

                            if (richiesta.Categoria == 0 || richiesta.Sede == 0 || richiesta.Certificato == 0)
                            {
                                messaggioVideo = "ATTENZIONE! Non è individuabile la pensione vigente per la domanda.";
                                return;
                            }
                        }
                        else
                            inseguiPensione = false;
                    }
                    else
                        inseguiPensione = false;
                }
            }
            if (!isNuovoTracciato)
                NormalizzaAreaToDB(AreaPrelievo, richiesta, out risposta, out messaggioVideo);
            else
                NormalizzaAreaToDBNew(AreaPrelievoNew, richiesta, out risposta, out messaggioVideo);
        }

        public static void PrelevaGP4(RichiestaPrelievo richiesta, out RispostaPrelievo risposta, out string messaggioVideo)
        {
            risposta = null;
            messaggioVideo = "";
            Data.GAIN AreaPrelievo = null;
            ValorizzaAreaPrelievo(richiesta, out AreaPrelievo, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
                return;
            Guid guid = Guid.NewGuid();
            GestioneLogSoap.SalvaLogSoap(AreaPrelievo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.IN, string.Empty, guid);
            EseguiPrelievo(AreaPrelievo);

            if (AreaPrelievo.HasError)
                GestioneLogSoap.SalvaLogSoap(AreaPrelievo.Messaggio, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.OUT, string.Empty, guid);
            else
                GestioneLogSoap.SalvaLogSoap(AreaPrelievo, Utility.Servizio.SrvLiquidazioneAgo, Utility.MetodoServizio.GAIN, Utility.SOAPLogDirection.OUT, string.Empty, guid);

            ControllaEsitoPrelievo(AreaPrelievo, out messaggioVideo);
            if (!String.IsNullOrEmpty(messaggioVideo))
                return;
            NormalizzaGP4ToDB(AreaPrelievo, out risposta);
        }
        #endregion public members

        #region private members
        private static void ValorizzaAreaPrelievo(RichiestaPrelievo richiesta, out Data.GAIN AreaPrelievo, out string messaggioVideo)
        {
            AreaPrelievo = null;
            messaggioVideo = "";
            if (richiesta.Sede == 0 || richiesta.SedeOperatore == 0 || richiesta.Categoria == 0 ||
                richiesta.Certificato == 0)
            {
                messaggioVideo = "Area richiesta non valorizzata correttamente";
                return;
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(richiesta.SedeOperatore.ToString().PadLeft(4, '0') + richiesta.CentroOperativoOperatore.ToString().PadLeft(2, '0'));
            string tipoRichiesta = "";
            switch (richiesta.TipoDomanda)
            {
                case TipoDomanda.Superstiti:
                    tipoRichiesta = "51";
                    break;
                case TipoDomanda.Ricostituzione:
                case TipoDomanda.Ripristino:
                case TipoDomanda.RipristinoSuperstiti:
                case TipoDomanda.Riliquidazione:
                case TipoDomanda.RiliquidazioneSuperstiti:
                    tipoRichiesta = "53";
                    break;
                default:
                    break;
            }

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            DateTime dataSistema = Utility.DataSistemaAgo;
            int annoCompetenza = 0;
            int annoComp = 0;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievo", out ctrl);

            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.AGO, out annoComp);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno di competenza
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo l'anno a 0
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                    annoCompetenza = dataSistema.Year + 1;
                else
                    annoCompetenza = annoComp;
            }

            AreaPrelievo = new INPS.Pensioni.LiquidazioneAgo.Data.GAIN(tipoRichiesta, richiesta.Categoria.ToString().PadLeft(3, '0'), richiesta.Sede,
                richiesta.Certificato, annoCompetenza);
        }

        private static void ValorizzaAreaPrelievoNew(RichiestaPrelievo richiesta, out Data.GAIN_New AreaPrelievo, out string messaggioVideo)
        {
            AreaPrelievo = null;
            messaggioVideo = "";
            if (richiesta.Sede == 0 || richiesta.SedeOperatore == 0 || richiesta.Categoria == 0 ||
                richiesta.Certificato == 0)
            {
                messaggioVideo = "Area richiesta non valorizzata correttamente";
                return;
            }
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = Utility.GetOfficeByAspnCode(richiesta.SedeOperatore.ToString().PadLeft(4, '0') + richiesta.CentroOperativoOperatore.ToString().PadLeft(2, '0'));
            string tipoRichiesta = "";
            switch (richiesta.TipoDomanda)
            {
                case TipoDomanda.Superstiti:
                    tipoRichiesta = "51";
                    break;
                case TipoDomanda.Ricostituzione:
                case TipoDomanda.Ripristino:
                case TipoDomanda.RipristinoSuperstiti:
                case TipoDomanda.Riliquidazione:
                case TipoDomanda.RiliquidazioneSuperstiti:
                    tipoRichiesta = "53";
                    break;
                default:
                    break;
            }

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoInterregno = null;
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            DateTime dataSistema = Utility.DataSistemaAgo;
            int annoCompetenza = 0;
            int annoComp = 0;

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataInizioInterregno", out controlloDinamicoInterregno);
            DateTime dataInizioInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("DataFineInterregno", out controlloDinamicoInterregno);
            DateTime dataFineInterregno = Utility.DataFromString(controlloDinamicoInterregno.ValoreControllo, Utility.FormatoData.AAAAmmGG).GetValueOrDefault();

            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ValorizzaAnnoCompetenzaPrelievo", out ctrl);

            GestioneControlliDinamici.GetAnnoCompetenza(Utility.TipoAppartenenza.AGO, out annoComp);

            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a SI e si tratta di una RIC o TRF rinnovata passo l'anno attuale + 1 se no passo l'anno attuale
            // se la chiave ValorizzaAnnoCompetenzaPrelievo è a NO passo l'anno a 0
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (richiesta.TipoDomanda == TipoDomanda.Ricostituzione && dataSistema != null && dataInizioInterregno != null && dataFineInterregno != null
                    && Utility.DataSuccessivaA(dataSistema, dataInizioInterregno) && !Utility.DataStrettamenteSuccessivaA(dataSistema, dataFineInterregno))
                    annoCompetenza = dataSistema.Year + 1;
                else
                    annoCompetenza = annoComp;
            }

            AreaPrelievo = new INPS.Pensioni.LiquidazioneAgo.Data.GAIN_New(tipoRichiesta, richiesta.Categoria.ToString().PadLeft(3, '0'), richiesta.Sede,
                richiesta.Certificato, annoCompetenza);
        }

        private static void EseguiPrelievo(Data.GAIN AreaPrelievo)
        {
            AreaPrelievo.Invoke();
        }

        private static void EseguiPrelievoNuovoTracciato(Data.GAIN_New AreaPrelievo)
        {
            AreaPrelievo.Invoke();
        }

        private static void ControllaEsitoPrelievo(Data.GAIN AreaPrelievo, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (AreaPrelievo.HasError)
            {
                if (!String.IsNullOrEmpty(AreaPrelievo.Messaggio))
                    messaggioVideo = AreaPrelievo.Messaggio;
                else if (AreaPrelievo.Response != null)
                {
                    if (AreaPrelievo.Response.Controllo.COD_RIT == "02")
                    {
                        messaggioVideo = "ERRORE RILEVATO DAL PGM PDBCOMXE - ";
                        if (!String.IsNullOrEmpty(AreaPrelievo.Response.Controllo.RETCODE.ToString()))
                            messaggioVideo += "CODICE RITORNO " + AreaPrelievo.Response.Controllo.RETCODE.ToString();
                        if (!String.IsNullOrEmpty(AreaPrelievo.Response.Controllo.TIPO_RETCODE))
                            messaggioVideo += "TIPO CODICE RITORNO " + AreaPrelievo.Response.Controllo.TIPO_RETCODE;
                        if (!String.IsNullOrEmpty(AreaPrelievo.Response.Controllo.TIPO_ACCESSO))
                            messaggioVideo += "TIPO ACCESSO " + AreaPrelievo.Response.Controllo.TIPO_ACCESSO;
                        if (!String.IsNullOrEmpty(AreaPrelievo.Response.Controllo.TAB_ERRORE))
                            messaggioVideo += "TAB ERRORE " + AreaPrelievo.Response.Controllo.TAB_ERRORE;
                        if (!String.IsNullOrEmpty(AreaPrelievo.Response.Controllo.PGM_ERRORE))
                            messaggioVideo += "PGM ERRORE " + AreaPrelievo.Response.Controllo.PGM_ERRORE;
                    }
                    else if (AreaPrelievo.Response.Controllo.COD_RIT == "07")
                    {
                        messaggioVideo = "ERRORE RILEVATO DALLA GAINMAIN - ";
                        if (!String.IsNullOrEmpty(AreaPrelievo.Response.Controllo.TAB_ERRORE))
                            messaggioVideo += "COD ERRORE: " + AreaPrelievo.Response.Controllo.TAB_ERRORE;
                        if (!String.IsNullOrEmpty(AreaPrelievo.Response.Controllo.DESC_ERRORE))
                            messaggioVideo += " - DESCRIZIONE: " + AreaPrelievo.Response.Controllo.DESC_ERRORE;
                    }
                    else
                    {
                        GestioneErroriPrelievo.ErroriPrelievo errore = null;
                        GestioneErroriPrelievo.GetErroriPrelievo(AreaPrelievo.Response.Controllo.COD_RIT, Utility.TipoAppartenenza.AGO, out errore);
                        if (errore != null)
                            messaggioVideo = errore.Descrizione;
                        else
                            messaggioVideo = "ERRORE PROCEDURA GAIN - CODICE ERRORE " + AreaPrelievo.Response.Controllo.COD_RIT;
                    }
                }
            }
        }

        private static void ControllaEsitoPrelievoNuovoTracciato(Data.GAIN_New AreaPrelievo, out string messaggioVideo)
        {
            messaggioVideo = "";
            if (AreaPrelievo.HasError)
            {
                if (!String.IsNullOrEmpty(AreaPrelievo.Messaggio))
                    messaggioVideo = AreaPrelievo.Messaggio;
                else if (AreaPrelievo.ResponseNew != null)
                {
                    if (AreaPrelievo.ResponseNew.Controllo.COD_RIT == "02")
                    {
                        messaggioVideo = "ERRORE RILEVATO DAL PGM PDBCOMXE - ";
                        if (!String.IsNullOrEmpty(AreaPrelievo.ResponseNew.Controllo.RETCODE.ToString()))
                            messaggioVideo += "CODICE RITORNO " + AreaPrelievo.ResponseNew.Controllo.RETCODE.ToString();
                        if (!String.IsNullOrEmpty(AreaPrelievo.ResponseNew.Controllo.TIPO_RETCODE))
                            messaggioVideo += "TIPO CODICE RITORNO " + AreaPrelievo.ResponseNew.Controllo.TIPO_RETCODE;
                        if (!String.IsNullOrEmpty(AreaPrelievo.ResponseNew.Controllo.TIPO_ACCESSO))
                            messaggioVideo += "TIPO ACCESSO " + AreaPrelievo.ResponseNew.Controllo.TIPO_ACCESSO;
                        if (!String.IsNullOrEmpty(AreaPrelievo.ResponseNew.Controllo.TAB_ERRORE))
                            messaggioVideo += "TAB ERRORE " + AreaPrelievo.ResponseNew.Controllo.TAB_ERRORE;
                        if (!String.IsNullOrEmpty(AreaPrelievo.ResponseNew.Controllo.PGM_ERRORE))
                            messaggioVideo += "PGM ERRORE " + AreaPrelievo.ResponseNew.Controllo.PGM_ERRORE;
                    }
                    else if (AreaPrelievo.ResponseNew.Controllo.COD_RIT == "07")
                    {
                        messaggioVideo = "ERRORE RILEVATO DALLA GAINMAIN - ";
                        if (!String.IsNullOrEmpty(AreaPrelievo.ResponseNew.Controllo.TAB_ERRORE))
                            messaggioVideo += "COD ERRORE: " + AreaPrelievo.ResponseNew.Controllo.TAB_ERRORE;
                        if (!String.IsNullOrEmpty(AreaPrelievo.ResponseNew.Controllo.DESC_ERRORE))
                            messaggioVideo += " - DESCRIZIONE: " + AreaPrelievo.ResponseNew.Controllo.DESC_ERRORE;
                    }
                    else
                    {
                        GestioneErroriPrelievo.ErroriPrelievo errore = null;
                        GestioneErroriPrelievo.GetErroriPrelievo(AreaPrelievo.ResponseNew.Controllo.COD_RIT, Utility.TipoAppartenenza.AGO, out errore);
                        if (errore != null)
                            messaggioVideo = errore.Descrizione;
                        else
                            messaggioVideo = "ERRORE PROCEDURA GAIN - CODICE ERRORE " + AreaPrelievo.ResponseNew.Controllo.COD_RIT;
                    }
                }
            }
        }

        private static void NormalizzaAreaToDB(Data.GAIN AreaPrelievo, RichiestaPrelievo richiesta, out RispostaPrelievo risposta, out string errore)
        {
            errore = null;
            risposta = new RispostaPrelievo();

            List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
            List<GestioneDecodifica.TipoCalcolo> listaTipoCalcolo = null;
            TipoDomanda tipoDomanda = richiesta.TipoDomanda;
            TipoRicostituzione tipoRicostituzione = richiesta.TipoRicostituzione;
            risposta.CategoriaPensione = richiesta.Categoria;
            risposta.SedePensione = richiesta.Sede;
            risposta.CertificatoPensione = richiesta.Certificato;

            List<string> categorieENPALS = new List<string> { "0201", "0202", "0203", "0204", "0205", "0206", "0207", "0208", "0209", "0210", "0211", "0212" };
            List<string> categorieSpacchettamentoENPALS = new List<string> { "0203", "0206", "0209", "0212" };
            List<string> categorieCumulo = new List<string> { "0170", "0171", "0172" };
            List<string> categorieTot = new List<string> { "0070", "0071", "0072" };
            string categoriaFromHost = AreaPrelievo.Response.DatiGenerici != null ? AreaPrelievo.Response.DatiGenerici.T_GP1AB01_V.Trim().ToUpperInvariant().PadLeft(4, '0') : null;

            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Pensionato != null)
            {
                risposta.CodiceFiscale = AreaPrelievo.Response.Pensionato.T_GP3CB08T_V;
            }
            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Istruttoria != null)
            {
                if (AreaPrelievo.Response.Istruttoria.T_GP1AD01_OA_V != 0 && AreaPrelievo.Response.Istruttoria.T_GP1AD01_OM_V != 0)
                {
                    //Se la categoria è IOCUM (171) potrebbe essere valorizzato un giorno diverso da 1 nel campo T_GP1AD02_V
                    if (risposta.CategoriaPensione == 171 && AreaPrelievo.Response.Istruttoria.T_GP1AD02_V != 0)
                        risposta.DataDecorrenza = new DateTime(AreaPrelievo.Response.Istruttoria.T_GP1AD01_OA_V, AreaPrelievo.Response.Istruttoria.T_GP1AD01_OM_V, AreaPrelievo.Response.Istruttoria.T_GP1AD02_V);
                    else
                        risposta.DataDecorrenza = new DateTime(AreaPrelievo.Response.Istruttoria.T_GP1AD01_OA_V, AreaPrelievo.Response.Istruttoria.T_GP1AD01_OM_V, 1);
                }
            }
            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.DatiNuovi != null)
            {
                //Per le ricostituzioni la cittadinanza deve essere inserita a mano dall'operatore
                if (!string.IsNullOrEmpty(AreaPrelievo.Response.DatiNuovi.T_GP1AXBA))
                {
                    if (listaStatiEsteri == null)
                        GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);

                    if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
                    {
                        string app = AreaPrelievo.Response.DatiNuovi.T_GP1AXBA == "I" ? "ITA" : AreaPrelievo.Response.DatiNuovi.T_GP1AXBA;
                        GestioneDecodifica.StatoEstero statoEstero = listaStatiEsteri.Find(x => x.Sigla == app);
                        if (statoEstero != null)
                        {
                            risposta.Cittadinanza = !string.IsNullOrEmpty(statoEstero.CodCatastale) ? statoEstero.CodCatastale.Trim() : string.Empty;
                        }
                    }
                }
            }
            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Pagamento != null)
            {
                risposta.IsRataEstratta = AreaPrelievo.Response.Pagamento.T_GP1ALZ5_V != 0;
            }

            //Rivista Gestione del ddlTipoPensione - 21/09/2020
            GestioneDecodifica.GetTipoCalcolo(out listaTipoCalcolo);
            if (listaTipoCalcolo != null && listaTipoCalcolo.Count > 0)
                listaTipoCalcolo = listaTipoCalcolo.FindAll(x => x.Tipologia == "AGO" && x.Tipo == (categorieENPALS.Contains(categoriaFromHost) ? "Enpals" : "Inps"));

            //Gestione dati retributivi BIS - decido se utilizzare area BIS o area originale
            //Al momento si utilizzerà sempre area originale
            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.DatiRetributiviBIS != null)
            {
                //GestioneControlliDinamici.ControlloDinamico ctrl = null;
                //GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRetributiviAgoBis", out ctrl);
                //if (ctrl == null || ctrl.ValoreControllo != "SI")
                //{
                AreaPrelievo.Response.DatiRetributiviBIS = null;
                //}
            }
            #region datiPensione
            GestionePensione.DatiPensione datiPensione = null;
            bool? enteIstruttoreExInpdap;
            MappingDaHost.ValorizzaDatiPensione(AreaPrelievo, tipoDomanda, categorieSpacchettamentoENPALS, categorieENPALS, out datiPensione, out enteIstruttoreExInpdap);
            risposta.DatiPensione = datiPensione;
            #endregion datiPensione

            #region datiIstruttoria
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneEnpals.DatiEnpals datiEnpals = null;
            MappingDaHost.ValorizzaDatiIstruttoria(AreaPrelievo, tipoDomanda, tipoRicostituzione, categorieENPALS, ref datiPensione, out datiEnpals, out datiIstruttoria);
            risposta.DatiIstruttoria = datiIstruttoria;
            risposta.DatiENPALS = datiEnpals;
            #endregion datiIstruttoria

            #region datiPagamento
            GestionePagamento.DatiPagamento datiPagamento = null;
            MappingDaHost.ValorizzaDatiPagamento(AreaPrelievo, tipoDomanda, out datiPagamento);
            risposta.DatiPagamento = datiPagamento;
            #endregion datiPagamento

            #region listaFamiliari
            List<Entity.DatiFamiliari> listaFamiliari = null;
            MappingDaHost.ValorizzaDatiFamiliare(AreaPrelievo, tipoDomanda, categorieSpacchettamentoENPALS, categoriaFromHost, out listaFamiliari);
            risposta.ListaFamiliari = listaFamiliari;
            #endregion listaFamiliari

            #region datiDanteCausa
            MappingDaHost.DatiAnagDanteCausa datiAnagDanteCausa = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            //Rivista Gestione del ddlTipoPensione - 21/09/2020
            MappingDaHost.ValorizzaDatiDanteCausa(AreaPrelievo, ref listaStatiEsteri, listaTipoCalcolo, out datiAnagDanteCausa, out datiDanteCausa);
            risposta.DatiDanteCausa = datiDanteCausa;
            risposta.DatiAnagDanteCausa = datiAnagDanteCausa;
            #endregion datiDanteCausa

            #region listaResidenzeEstere
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            MappingDaHost.ValorizzaDatiResidenzeEstere(AreaPrelievo, listaStatiEsteri, out listaResidenzeEstere);
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

            #region listaCalcoloRetributivo
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo = null;
            GestioneCalcolo.DatiCalcoloRetributivoENPAL calcoloRetributivoENPALS = null;
            List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> listaDatiRetributiviINPGI = null;
            MappingDaHost.ValorizzaDatiCalcoloRetributivo(AreaPrelievo, categorieENPALS, tipoRicostituzione, ref datiPensione, out listaCalcoloRetributivo, out calcoloRetributivoENPALS, out listaDatiRetributiviINPGI);
            GestioneRetribuzione(richiesta, ref listaCalcoloRetributivo);
            risposta.ListaCalcoloRetributivo = listaCalcoloRetributivo;
            risposta.CalcoloRetributivoENPALS = calcoloRetributivoENPALS;
            risposta.ListaDatiRetributiviINPGI = listaDatiRetributiviINPGI;
            #endregion listaCalcoloRetributivo

            #region listaCalcoloContributivo
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo = null;
            GestioneCalcolo.DatiCalcoloContributivoENPAL calcoloContributivoENPALS = null;
            List<Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS> listaSuppRecordENPALS = null;
            List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> listaQuotaFondoIntegrativo = null;
            List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> listaDatiContributiviINPGI = null;
            MappingDaHost.ValorizzaDatiCalcoloContributivo(AreaPrelievo, categorieENPALS, tipoRicostituzione, richiesta.Tipo, ref datiEnpals, ref datiPensione, out listaCalcoloContributivo, out calcoloContributivoENPALS, out listaSuppRecordENPALS, out listaQuotaFondoIntegrativo, out listaDatiContributiviINPGI);
            risposta.ListaCalcoloContributivo = listaCalcoloContributivo;
            risposta.CalcoloContributivoENPALS = calcoloContributivoENPALS;
            risposta.ListaSuppRecordENPALS = listaSuppRecordENPALS;
            risposta.ListaQuotaFondoIntegrativo = listaQuotaFondoIntegrativo;
            risposta.ListaDatiContributiviINPGI = listaDatiContributiviINPGI;
            #endregion listaCalcoloContributivo

            #region listaQuotePensione
            List<GestioneCalcolo.QuotePensione> listaQuotePensione = null;
            List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattuali;
            MappingDaHost.ValorizzaDatiQuotePensione(AreaPrelievo, out listaQuotePensione, out listaQuoteMiglioramentiContrattuali);
            risposta.ListaQuotePensione = listaQuotePensione;
            risposta.ListaQuoteMiglioramentiContrattuali = listaQuoteMiglioramentiContrattuali;
            #endregion listaQuotePensione

            #region listaTrattenuteQuotePensione
            List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenute = null;
            MappingDaHost.ValorizzaDatiTrattenuteQuotePensione(AreaPrelievo, out listaTrattenute);
            risposta.ListaTrattenuteQuotePensione = listaTrattenute;
            #endregion listaTrattenuteQuotePensione

            #region datiDetrazioni
            //tolta la condizione preesistente poichè è stata aggiunta la condizione che deve essere diverso da ricostituzione e riapertura in MappingVersoHost.cs
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            MappingDaHost.ValorizzaDatiDetrazioni(AreaPrelievo, out datiDetrazioni);
            risposta.DatiDetrazioni = datiDetrazioni;
            #endregion datiDetrazioni

            #region datiSindacato
            GestionePensione.DatiSindacato datiSindacato = null;
            MappingDaHost.ValorizzaDatiSindacato(AreaPrelievo, out datiSindacato);
            risposta.DatiSindacato = datiSindacato;
            #endregion datiSindacato

            #region listaSupplementi
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            List<Liquidazione.BLCommon.Entity.DatiSupplementiENPALS> listaSupplementiEnpals = null;
            List<Liquidazione.BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiCumulo = null;
            MappingDaHost.ValorizzaDatiSupplementi(AreaPrelievo, tipoDomanda, categorieENPALS, categorieCumulo, categorieTot, datiPensione, datiDanteCausa, out listaSupplementi, out listaSupplementiEnpals, ref listaSuppRecordENPALS,
                ref listaSupplementiCumulo, tipoRicostituzione, richiesta.Tipo, richiesta.Prodotto, out errore);
            risposta.ListaSupplementi = listaSupplementi;
            risposta.ListaSupplementiENPALS = listaSupplementiEnpals;
            risposta.ListaSupplementiCumulo = listaSupplementiCumulo;

            if (!string.IsNullOrEmpty(errore))
                return;
            #endregion listaSupplementi

            #region datiSupplementiBase
            INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase datiSupplementiBase = null;
            MappingDaHost.ValorizzaDatiSupplementiBase(AreaPrelievo, categorieENPALS, out datiSupplementiBase);
            risposta.DatiSupplementiBase = datiSupplementiBase;
            #endregion datiSupplementiBase

            #region datiIntegrazioneArt11
            GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11 = null;
            MappingDaHost.ValorizzaDatiIntegrazioneArt11(AreaPrelievo, out datiIntegrazioneArt11);
            risposta.DatiIntegrazioneArt11 = datiIntegrazioneArt11;
            #endregion datiIntegrazioneArt11

            #region datiEliminazione
            GestionePensione.DatiEliminazione datiEliminazione = null;
            MappingDaHost.ValorizzaDatiEliminazione(AreaPrelievo, tipoDomanda, out datiEliminazione);
            risposta.DatiEliminazione = datiEliminazione;
            #endregion datiEliminazione

            #region datiMaggiorazioniBenefici
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            MappingDaHost.ValorizzaDatiMaggiorazioni(AreaPrelievo, ref datiPensione, out datiMaggiorazioniBenefici);
            risposta.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            #endregion datiMaggiorazioniBenefici

            #region datiPensioniDatiGenerici
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            MappingDaHost.ValorizzaPensioniDatiGenerici(AreaPrelievo, tipoDomanda, enteIstruttoreExInpdap, out datiPensioniDatiGenerici);
            risposta.DatiPensioniDatiGenerici = datiPensioniDatiGenerici;
            #endregion datiPensioniDatiGenerici

            #region datiBititolarità
            List<GestioneAltrePensioni.AltraPensione> listaBititolarita = null;
            MappingDaHost.ValorizzaDatiBititolarita(AreaPrelievo, out listaBititolarita);
            risposta.ListaBititolarita = listaBititolarita;
            #endregion datiBititolarità

            #region datiInailInabilita
            List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaInail = null;
            GestionePensioneInailInabilita.DatiInabilita datiInabilita = null;
            MappingDaHost.ValorizzaDatiInabilitaINAIL(AreaPrelievo, out listaInail, out datiInabilita);
            risposta.ListaInail = listaInail;
            risposta.DatiInabilita = datiInabilita;
            #endregion datiInailInabilita

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

            #region datiRedditiSentenza495_93
            List<GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditiSentenza495_93 = null;
            MappingDaHost.ValorizzaRedditiSentenza495_93(AreaPrelievo, out listaDatiRedditiSentenza495_93);
            risposta.ListaDatiRedditiSentenza495_93 = listaDatiRedditiSentenza495_93;
            #endregion datiRedditiSentenza495_93

            #region datiBeneficioVittimeTerrorismo
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            bool isGP1AC01Valorizzato = false;
            MappingDaHost.ValorizzaDatiBeneficioVittimeTerrorismo(AreaPrelievo, out datiBeneficioVittimeTerrorismo, out isGP1AC01Valorizzato);
            risposta.DatiBeneficioVittimeTerrorismo = datiBeneficioVittimeTerrorismo;
            risposta.IsGP1AC01Valorizzato = isGP1AC01Valorizzato;
            #endregion datiBeneficioVittimeTerrorismo

            #region datiCalcoloVittimeTerrorismo
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = null;
            MappingDaHost.ValorizzaDatiCalcoloVittimeTerrorismo(AreaPrelievo, out listaDatiCalcoloVittimeTerrorismo);
            risposta.ListaDatiCalcoloVittimeTerrorismo = listaDatiCalcoloVittimeTerrorismo;
            #endregion datiCalcoloVittimeTerrosimo

            #region AventiDiritto e Periodi
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDiritto = null;
            MappingDaHost.ValorizzaAventiDiritto_Periodi(AreaPrelievo, out listaDatiAventiDiritto);
            risposta.ListaDatiAventiDiritto = listaDatiAventiDiritto;
            #endregion AventiDiritto e Periodi

            #region datiSentenzaArt4
            List<GestioneSentenzaArt4.DatiSentenzaArt4> listaDatiSentenzaArt4 = null;
            MappingDaHost.ValorizzaSentenzaArt4(AreaPrelievo, out listaDatiSentenzaArt4);
            risposta.ListaDatiSentenzaArt4 = listaDatiSentenzaArt4;
            #endregion datiSentenzaArt4

            #region datiSentenze
            List<GestioneSentenze.DatiSentenze> listaDatiSentenze = null;
            MappingDaHost.ValorizzaSentenze(AreaPrelievo, out listaDatiSentenze);
            risposta.ListaDatiSentenze = listaDatiSentenze;
            #endregion datiSentenze

            if (!(tipoDomanda == TipoDomanda.Ricostituzione && (categoriaFromHost == "0243" || categoriaFromHost == "0244" || categoriaFromHost == "0245")))
                Utility.GetTipoCalcoloFromDatiHost(ref datiPensione, listaCalcoloRetributivo, listaCalcoloContributivo, calcoloRetributivoENPALS, calcoloContributivoENPALS, datiEnpals);

            // Solo per le domande di reversibilità, se non sono riuscito a calcolare il tipo calcolo, perchè mancano effettivamente i dati calcolo allora lo recupero da GP1AF03
            if (!datiPensione.TipoCalcolo.HasValue && tipoDomanda == TipoDomanda.Superstiti)
            {
                if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Istruttoria != null)
                {
                    if (categorieENPALS.Contains(categoriaFromHost) || (AreaPrelievo.Response.DatiGenerici.T_GP1AB02_V.ToString() == "9933" && (risposta.SedePensione == 801 || risposta.SedePensione == 802)))
                    {
                        risposta.GP1AF03 = AreaPrelievo.Response.Istruttoria.T_GP1AF03_V;
                    }
                }
            }

            if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.Istruttoria != null)
            {
                risposta.GP1ALB1 = AreaPrelievo.Response.Istruttoria.T_GP1ALB1;
                risposta.GP1AXE3 = AreaPrelievo.Response.Istruttoria.T_GP1AXE3;

                if (tipoDomanda == TipoDomanda.Ricostituzione && !categorieENPALS.Contains(categoriaFromHost))
                {
                    risposta.GP1AZ11F = AreaPrelievo.Response.Istruttoria.T_GP1AZ11F;
                }
            }

            if (categoriaFromHost == "0199" || categoriaFromHost == "0029" || categoriaFromHost == "0129")
            {
                if (AreaPrelievo.Response != null && AreaPrelievo.Response.PannelloContributivo != null)
                {
                    if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03 != null && AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.Count > 0)
                    {
                        if (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05 != null &&
                           (AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "E" ||
                            AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "L1"))
                        {
                            risposta.GP2BB05 = AreaPrelievo.Response.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant();
                        }
                    }
                }
                //per le categorie 29 (VOESO), 129 (VESO29) e 199 (VESO92) se il giorno della scadenza assegno è valorizzato setto il campo IsScadenzaAssegnoConGiorno
                if (AreaPrelievo.Response.Istruttoria != null && AreaPrelievo.Response.Istruttoria.T_GP1AG02G > 0)
                    risposta.IsScadenzaAssegnoConGiorno = true;
                else
                    risposta.IsScadenzaAssegnoConGiorno = false;

                if (AreaPrelievo != null && AreaPrelievo.Response != null && AreaPrelievo.Response.DatiNuovi != null && categoriaFromHost == "0029")
                {
                    risposta.GP1AV91H = AreaPrelievo.Response.DatiNuovi.T_GP1AV91H;
                }
            }

            if (AreaPrelievo.Response.Coda != null && AreaPrelievo.Response.Coda.AreaDati2021 != null && AreaPrelievo.Response.Coda.AreaDati2021.T_GP1AJTIPCUM != null)
            {
                risposta.GP1AJTIPCUM = AreaPrelievo.Response.Coda.AreaDati2021.T_GP1AJTIPCUM;
            }
        }

        private static void NormalizzaAreaToDBNew(Data.GAIN_New AreaPrelievo, RichiestaPrelievo richiesta, out RispostaPrelievo risposta, out string errore)
        {
            errore = null;
            risposta = new RispostaPrelievo();

            List<GestioneDecodifica.StatoEstero> listaStatiEsteri = null;
            List<GestioneDecodifica.TipoCalcolo> listaTipoCalcolo = null;
            TipoDomanda tipoDomanda = richiesta.TipoDomanda;
            TipoRicostituzione tipoRicostituzione = richiesta.TipoRicostituzione;
            risposta.CategoriaPensione = richiesta.Categoria;
            risposta.SedePensione = richiesta.Sede;
            risposta.CertificatoPensione = richiesta.Certificato;

            List<string> categorieENPALS = new List<string> { "0201", "0202", "0203", "0204", "0205", "0206", "0207", "0208", "0209", "0210", "0211", "0212" };
            List<string> categorieSpacchettamentoENPALS = new List<string> { "0203", "0206", "0209", "0212" };
            List<string> categorieCumulo = new List<string> { "0170", "0171", "0172" };
            List<string> categorieTot = new List<string> { "0070", "0071", "0072" };
            string categoriaFromHost = AreaPrelievo.ResponseNew.DatiGenerici != null ? AreaPrelievo.ResponseNew.DatiGenerici.T_GP1AB01_V.Trim().ToUpperInvariant().PadLeft(4, '0') : null;

            if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.Pensionato != null)
            {
                risposta.CodiceFiscale = AreaPrelievo.ResponseNew.Pensionato.T_GP3CB08T_V;
            }
            if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.Istruttoria != null)
            {
                if (AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD01_OA_V != 0 && AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD01_OM_V != 0)
                {
                    //Se la categoria è IOCUM (171) potrebbe essere valorizzato un giorno diverso da 1 nel campo T_GP1AD02_V
                    if (risposta.CategoriaPensione == 171 && AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD02_V != 0)
                        risposta.DataDecorrenza = new DateTime(AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD01_OA_V, AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD01_OM_V, AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD02_V);
                    else
                        risposta.DataDecorrenza = new DateTime(AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD01_OA_V, AreaPrelievo.ResponseNew.Istruttoria.T_GP1AD01_OM_V, 1);
                }
            }
            if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.DatiNuovi != null)
            {
                //Per le ricostituzioni la cittadinanza deve essere inserita a mano dall'operatore
                if (!string.IsNullOrEmpty(AreaPrelievo.ResponseNew.DatiNuovi.T_GP1AXBA))
                {
                    if (listaStatiEsteri == null)
                        GestioneDecodifica.GetStatiEsteri(out listaStatiEsteri);

                    if (listaStatiEsteri != null && listaStatiEsteri.Count > 0)
                    {
                        string app = AreaPrelievo.ResponseNew.DatiNuovi.T_GP1AXBA == "I" ? "ITA" : AreaPrelievo.ResponseNew.DatiNuovi.T_GP1AXBA;
                        GestioneDecodifica.StatoEstero statoEstero = listaStatiEsteri.Find(x => x.Sigla == app);
                        if (statoEstero != null)
                        {
                            risposta.Cittadinanza = !string.IsNullOrEmpty(statoEstero.CodCatastale) ? statoEstero.CodCatastale.Trim() : string.Empty;
                        }
                    }
                }
            }
            if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.Pagamento != null)
            {
                risposta.IsRataEstratta = AreaPrelievo.ResponseNew.Pagamento.T_GP1ALZ5_V != 0;
            }

            //Rivista Gestione del ddlTipoPensione - 21/09/2020
            GestioneDecodifica.GetTipoCalcolo(out listaTipoCalcolo);
            if (listaTipoCalcolo != null && listaTipoCalcolo.Count > 0)
                listaTipoCalcolo = listaTipoCalcolo.FindAll(x => x.Tipologia == "AGO" && x.Tipo == (categorieENPALS.Contains(categoriaFromHost) ? "Enpals" : "Inps"));

            //Gestione dati retributivi BIS - decido se utilizzare area BIS o area originale
            //Al momento si utilizzerà sempre area originale
            if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.DatiRetributiviBIS != null)
            {
                //GestioneControlliDinamici.ControlloDinamico ctrl = null;
                //GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRetributiviAgoBis", out ctrl);
                //if (ctrl == null || ctrl.ValoreControllo != "SI")
                //{
                AreaPrelievo.ResponseNew.DatiRetributiviBIS = null;
                //}
            }
            #region datiPensione
            GestionePensione.DatiPensione datiPensione = null;
            bool? enteIstruttoreExInpdap;
            MappingDaHostNew.ValorizzaDatiPensione(AreaPrelievo, tipoDomanda, categorieSpacchettamentoENPALS, categorieENPALS, out datiPensione, out enteIstruttoreExInpdap);
            risposta.DatiPensione = datiPensione;
            #endregion datiPensione

            #region datiIstruttoria
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneEnpals.DatiEnpals datiEnpals = null;
            MappingDaHostNew.ValorizzaDatiIstruttoria(AreaPrelievo, tipoDomanda, tipoRicostituzione, categorieENPALS, ref datiPensione, out datiEnpals, out datiIstruttoria);
            risposta.DatiIstruttoria = datiIstruttoria;
            risposta.DatiENPALS = datiEnpals;
            #endregion datiIstruttoria

            #region datiPagamento
            GestionePagamento.DatiPagamento datiPagamento = null;
            MappingDaHostNew.ValorizzaDatiPagamento(AreaPrelievo, tipoDomanda, out datiPagamento);
            risposta.DatiPagamento = datiPagamento;
            #endregion datiPagamento

            #region listaFamiliari
            List<Entity.DatiFamiliari> listaFamiliari = null;
            MappingDaHostNew.ValorizzaDatiFamiliare(AreaPrelievo, tipoDomanda, categorieSpacchettamentoENPALS, categoriaFromHost, out listaFamiliari);
            risposta.ListaFamiliari = listaFamiliari;
            #endregion listaFamiliari

            #region datiDanteCausa
            MappingDaHostNew.DatiAnagDanteCausa datiAnagDanteCausaNew = null;
            GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
            //Rivista Gestione del ddlTipoPensione - 21/09/2020
            MappingDaHostNew.ValorizzaDatiDanteCausa(AreaPrelievo, ref listaStatiEsteri, listaTipoCalcolo, out datiAnagDanteCausaNew, out datiDanteCausa);
            MappingDaHost.DatiAnagDanteCausa datiAnagDanteCausa = new MappingDaHost.DatiAnagDanteCausa();
            Utility.ValorizzaOggetti(datiAnagDanteCausaNew, datiAnagDanteCausa);
            risposta.DatiDanteCausa = datiDanteCausa;
            risposta.DatiAnagDanteCausa = datiAnagDanteCausa;
            #endregion datiDanteCausa

            #region listaResidenzeEstere
            List<GestioneAnagrafica.DatiResidenzaEstero> listaResidenzeEstere = null;
            MappingDaHostNew.ValorizzaDatiResidenzeEstere(AreaPrelievo, listaStatiEsteri, out listaResidenzeEstere);
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

            #region listaCalcoloRetributivo
            List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo = null;
            GestioneCalcolo.DatiCalcoloRetributivoENPAL calcoloRetributivoENPALS = null;
            List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> listaDatiRetributiviINPGI = null;
            MappingDaHostNew.ValorizzaDatiCalcoloRetributivo(AreaPrelievo, categorieENPALS, tipoRicostituzione, ref datiPensione, out listaCalcoloRetributivo, out calcoloRetributivoENPALS, out listaDatiRetributiviINPGI);
            GestioneRetribuzione(richiesta, ref listaCalcoloRetributivo);
            risposta.ListaCalcoloRetributivo = listaCalcoloRetributivo;
            risposta.CalcoloRetributivoENPALS = calcoloRetributivoENPALS;
            risposta.ListaDatiRetributiviINPGI = listaDatiRetributiviINPGI;
            #endregion listaCalcoloRetributivo

            #region listaCalcoloContributivo
            List<GestioneCalcolo.DatiCalcoloContributivo> listaCalcoloContributivo = null;
            GestioneCalcolo.DatiCalcoloContributivoENPAL calcoloContributivoENPALS = null;
            List<Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS> listaSuppRecordENPALS = null;
            List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> listaQuotaFondoIntegrativo = null;
            List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> listaDatiContributiviINPGI = null;
            MappingDaHostNew.ValorizzaDatiCalcoloContributivo(AreaPrelievo, categorieENPALS, tipoRicostituzione, richiesta.Tipo, ref datiEnpals, ref datiPensione, out listaCalcoloContributivo, out calcoloContributivoENPALS, out listaSuppRecordENPALS, out listaQuotaFondoIntegrativo, out listaDatiContributiviINPGI);
            risposta.ListaCalcoloContributivo = listaCalcoloContributivo;
            risposta.CalcoloContributivoENPALS = calcoloContributivoENPALS;
            risposta.ListaSuppRecordENPALS = listaSuppRecordENPALS;
            risposta.ListaQuotaFondoIntegrativo = listaQuotaFondoIntegrativo;
            risposta.ListaDatiContributiviINPGI = listaDatiContributiviINPGI;
            #endregion listaCalcoloContributivo

            #region listaQuotePensione
            List<GestioneCalcolo.QuotePensione> listaQuotePensione = null;
            List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> listaQuoteMiglioramentiContrattuali;
            MappingDaHostNew.ValorizzaDatiQuotePensione(AreaPrelievo, out listaQuotePensione, out listaQuoteMiglioramentiContrattuali);
            risposta.ListaQuotePensione = listaQuotePensione;
            risposta.ListaQuoteMiglioramentiContrattuali = listaQuoteMiglioramentiContrattuali;
            #endregion listaQuotePensione

            #region listaTrattenuteQuotePensione
            List<GestioneCalcolo.TrattenuteQuotePensione> listaTrattenute = null;
            MappingDaHostNew.ValorizzaDatiTrattenuteQuotePensione(AreaPrelievo, out listaTrattenute);
            risposta.ListaTrattenuteQuotePensione = listaTrattenute;
            #endregion listaTrattenuteQuotePensione

            #region datiDetrazioni
            //tolta la condizione preesistente poichè è stata aggiunta la condizione che deve essere diverso da ricostituzione e riapertura in MappingVersoHost.cs
            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            MappingDaHostNew.ValorizzaDatiDetrazioni(AreaPrelievo, out datiDetrazioni);
            risposta.DatiDetrazioni = datiDetrazioni;
            #endregion datiDetrazioni

            #region datiSindacato
            GestionePensione.DatiSindacato datiSindacato = null;
            MappingDaHostNew.ValorizzaDatiSindacato(AreaPrelievo, out datiSindacato);
            risposta.DatiSindacato = datiSindacato;
            #endregion datiSindacato

            #region listaSupplementi
            List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> listaSupplementi = null;
            List<Liquidazione.BLCommon.Entity.DatiSupplementiENPALS> listaSupplementiEnpals = null;
            List<Liquidazione.BLCommon.Entity.DatiSupplementiCumulo> listaSupplementiCumulo = null;
            MappingDaHostNew.ValorizzaDatiSupplementi(AreaPrelievo, tipoDomanda, categorieENPALS, categorieCumulo, categorieTot, datiPensione, datiDanteCausa, out listaSupplementi, out listaSupplementiEnpals, ref listaSuppRecordENPALS,
                ref listaSupplementiCumulo, tipoRicostituzione, richiesta.Tipo, richiesta.Prodotto, out errore);
            risposta.ListaSupplementi = listaSupplementi;
            risposta.ListaSupplementiENPALS = listaSupplementiEnpals;
            risposta.ListaSupplementiCumulo = listaSupplementiCumulo;

            if (!string.IsNullOrEmpty(errore))
                return;
            #endregion listaSupplementi

            #region datiSupplementiBase
            INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase datiSupplementiBase = null;
            MappingDaHostNew.ValorizzaDatiSupplementiBase(AreaPrelievo, categorieENPALS, out datiSupplementiBase);
            risposta.DatiSupplementiBase = datiSupplementiBase;
            #endregion datiSupplementiBase

            #region datiIntegrazioneArt11
            GestioneIntegrazioneArt11.IntegrazioneArt11 datiIntegrazioneArt11 = null;
            MappingDaHostNew.ValorizzaDatiIntegrazioneArt11(AreaPrelievo, out datiIntegrazioneArt11);
            risposta.DatiIntegrazioneArt11 = datiIntegrazioneArt11;
            #endregion datiIntegrazioneArt11

            #region datiEliminazione
            GestionePensione.DatiEliminazione datiEliminazione = null;
            MappingDaHostNew.ValorizzaDatiEliminazione(AreaPrelievo, tipoDomanda, out datiEliminazione);
            risposta.DatiEliminazione = datiEliminazione;
            #endregion datiEliminazione

            #region datiMaggiorazioniBenefici
            INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici datiMaggiorazioniBenefici = null;
            if (tipoDomanda != TipoDomanda.Riliquidazione && tipoDomanda != TipoDomanda.RiliquidazioneSuperstiti)
            MappingDaHostNew.ValorizzaDatiMaggiorazioni(AreaPrelievo, ref datiPensione, out datiMaggiorazioniBenefici, tipoDomanda);
            risposta.DatiMaggiorazioniBenefici = datiMaggiorazioniBenefici;
            #endregion datiMaggiorazioniBenefici

            #region datiPensioniDatiGenerici
            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            MappingDaHostNew.ValorizzaPensioniDatiGenerici(AreaPrelievo, tipoDomanda, enteIstruttoreExInpdap, out datiPensioniDatiGenerici);
            risposta.DatiPensioniDatiGenerici = datiPensioniDatiGenerici;
            ServiceReferences.DatiPensioni.DatiTGP6Response response;
            string errori;
            
            string chiavePensione = richiesta.Categoria.ToString().PadLeft(3, '0') + richiesta.Sede.ToString().PadLeft(4, '0') + richiesta.Certificato.ToString().PadLeft(8, '0');
            if (Utility.IsDomandaINPDAI(datiPensione.SiglaCategoria) && Utility.IsDomandaRipristino(datiPensione).GetValueOrDefault() && Utility.DataStrettamenteSuccessivaA(new DateTime(1997, 01, 01), datiPensione.DecorrenzaOriginaria.GetValueOrDefault()) &&
                GestioneDatiPensioni.GetDatiTGP6ByChiavePensione(datiPensione.NDomus, chiavePensione, out response, out errori) && response != null && response.ListaDatiTGP6 != null)
                {
                   
                   var GP6KC04E = response.ListaDatiTGP6.FirstOrDefault(x => x.GP6KC01Z.Valore.Codice == "200312");
                   datiPensioniDatiGenerici.ImportoAl200312 = (GP6KC04E == null && GP6KC04E.GP6KC04E == null && GP6KC04E.GP6KC04E.Valore.Codice == null) ? null : Utility.StringToNullableDecimalPoint(GP6KC04E.GP6KC04E.Valore.Codice);
                }
            
            #endregion datiPensioniDatiGenerici

            #region datiBititolarità
            List<GestioneAltrePensioni.AltraPensione> listaBititolarita = null;
            MappingDaHostNew.ValorizzaDatiBititolarita(AreaPrelievo, out listaBititolarita);
            risposta.ListaBititolarita = listaBititolarita;
            #endregion datiBititolarità

            #region datiInailInabilita
            List<GestionePensioneInailInabilita.DatiPensioniINAIL> listaInail = null;
            GestionePensioneInailInabilita.DatiInabilita datiInabilita = null;
            MappingDaHostNew.ValorizzaDatiInabilitaINAIL(AreaPrelievo, out listaInail, out datiInabilita);
            risposta.ListaInail = listaInail;
            risposta.DatiInabilita = datiInabilita;
            #endregion datiInailInabilita

            #region datiOneri
            List<GestioneOneri.DatiOneri> listaDatiOneri = null;
            MappingDaHostNew.ValorizzaDatiOneri(AreaPrelievo, out listaDatiOneri, ref datiPensione, tipoDomanda);
            risposta.ListaDatiOneri = listaDatiOneri;
            #endregion datiOneri

            #region datiBeneficiParticolari
            List<GestioneBeneficiParticolari.DatiBeneficiParticolari> listaDatiBeneficiParticolari = null;
            MappingDaHostNew.ValorizzaDatiBeneficiParticolari(AreaPrelievo, out listaDatiBeneficiParticolari);
            risposta.ListaDatiBeneficiParticolari = listaDatiBeneficiParticolari;
            #endregion datiBeneficiParticolari

            #region datiRedditiSentenza495_93
            List<GestioneDanteCausa.DatiRedditoSentenza495_93> listaDatiRedditiSentenza495_93 = null;
            MappingDaHostNew.ValorizzaRedditiSentenza495_93(AreaPrelievo, out listaDatiRedditiSentenza495_93);
            risposta.ListaDatiRedditiSentenza495_93 = listaDatiRedditiSentenza495_93;
            #endregion datiRedditiSentenza495_93

            #region datiBeneficioVittimeTerrorismo
            GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo datiBeneficioVittimeTerrorismo = null;
            bool isGP1AC01Valorizzato = false;
            MappingDaHostNew.ValorizzaDatiBeneficioVittimeTerrorismo(AreaPrelievo, out datiBeneficioVittimeTerrorismo, out isGP1AC01Valorizzato);
            risposta.DatiBeneficioVittimeTerrorismo = datiBeneficioVittimeTerrorismo;
            risposta.IsGP1AC01Valorizzato = isGP1AC01Valorizzato;
            #endregion datiBeneficioVittimeTerrorismo

            #region datiCalcoloVittimeTerrorismo
            List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> listaDatiCalcoloVittimeTerrorismo = null;
            MappingDaHostNew.ValorizzaDatiCalcoloVittimeTerrorismo(AreaPrelievo, out listaDatiCalcoloVittimeTerrorismo);
            risposta.ListaDatiCalcoloVittimeTerrorismo = listaDatiCalcoloVittimeTerrorismo;
            #endregion datiCalcoloVittimeTerrosimo

            #region AventiDiritto e Periodi
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDiritto = null;
            MappingDaHostNew.ValorizzaAventiDiritto_Periodi(AreaPrelievo, out listaDatiAventiDiritto);
            risposta.ListaDatiAventiDiritto = listaDatiAventiDiritto;
            #endregion AventiDiritto e Periodi

            #region datiSentenzaArt4
            List<GestioneSentenzaArt4.DatiSentenzaArt4> listaDatiSentenzaArt4 = null;
            MappingDaHostNew.ValorizzaSentenzaArt4(AreaPrelievo, out listaDatiSentenzaArt4);
            risposta.ListaDatiSentenzaArt4 = listaDatiSentenzaArt4;
            #endregion datiSentenzaArt4

            #region datiSentenze
            List<GestioneSentenze.DatiSentenze> listaDatiSentenze = null;
            MappingDaHostNew.ValorizzaSentenze(AreaPrelievo, out listaDatiSentenze);
            risposta.ListaDatiSentenze = listaDatiSentenze;
            #endregion datiSentenze

            #region datiPrestazioniEstere
            List<GestioneContrib.StatoEsteroCumulo> listaStatiEsteriCumulo = null;
            MappingDaHostNew.ValorizzaDatiStatiEsteri(AreaPrelievo, out listaStatiEsteriCumulo);
            risposta.ListaStatiEsteri = listaStatiEsteriCumulo;
            #endregion datiPrestazioniEstere

            if (!(tipoDomanda == TipoDomanda.Ricostituzione && (categoriaFromHost == "0243" || categoriaFromHost == "0244" || categoriaFromHost == "0245")))
                Utility.GetTipoCalcoloFromDatiHost(ref datiPensione, listaCalcoloRetributivo, listaCalcoloContributivo, calcoloRetributivoENPALS, calcoloContributivoENPALS, datiEnpals);

            // Solo per le domande di reversibilità, se non sono riuscito a calcolare il tipo calcolo, perchè mancano effettivamente i dati calcolo allora lo recupero da GP1AF03
            if (!datiPensione.TipoCalcolo.HasValue && tipoDomanda == TipoDomanda.Superstiti)
            {
                if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.Istruttoria != null)
                {
                    if (categorieENPALS.Contains(categoriaFromHost) || (AreaPrelievo.ResponseNew.DatiGenerici.T_GP1AB02_V.ToString() == "9933" && (risposta.SedePensione == 801 || risposta.SedePensione == 802)))
                    {
                        risposta.GP1AF03 = AreaPrelievo.ResponseNew.Istruttoria.T_GP1AF03_V;
                    }
                }
            }

            if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.Istruttoria != null)
            {
                risposta.GP1ALB1 = AreaPrelievo.ResponseNew.Istruttoria.T_GP1ALB1;
                risposta.GP1AXE3 = AreaPrelievo.ResponseNew.Istruttoria.T_GP1AXE3;

                if (tipoDomanda == TipoDomanda.Ricostituzione && !categorieENPALS.Contains(categoriaFromHost))
                {
                    risposta.GP1AZ11F = AreaPrelievo.ResponseNew.Istruttoria.T_GP1AZ11F;
                }
            }

            if (categoriaFromHost == "0199" || categoriaFromHost == "0029" || categoriaFromHost == "0129")
            {
                if (AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.PannelloContributivo != null)
                {
                    if (AreaPrelievo.ResponseNew.PannelloContributivo.LISTT_GP2BB03 != null && AreaPrelievo.ResponseNew.PannelloContributivo.LISTT_GP2BB03.Count > 0)
                    {
                        if (AreaPrelievo.ResponseNew.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05 != null &&
                           (AreaPrelievo.ResponseNew.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "E" ||
                            AreaPrelievo.ResponseNew.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant() == "L1"))
                        {
                            risposta.GP2BB05 = AreaPrelievo.ResponseNew.PannelloContributivo.LISTT_GP2BB03.First().T_GP2BB05.Trim().ToUpperInvariant();
                        }
                    }
                }
                //per le categorie 29 (VOESO), 129 (VESO29) e 199 (VESO92) se il giorno della scadenza assegno è valorizzato setto il campo IsScadenzaAssegnoConGiorno
                if (AreaPrelievo.ResponseNew.Istruttoria != null && AreaPrelievo.ResponseNew.Istruttoria.T_GP1AG02G > 0)
                    risposta.IsScadenzaAssegnoConGiorno = true;
                else
                    risposta.IsScadenzaAssegnoConGiorno = false;

                if (AreaPrelievo != null && AreaPrelievo.ResponseNew != null && AreaPrelievo.ResponseNew.DatiNuovi != null && categoriaFromHost == "0029")
                {
                    risposta.GP1AV91H = AreaPrelievo.ResponseNew.DatiNuovi.T_GP1AV91H;
                }
            }

            if (AreaPrelievo.ResponseNew.Coda != null && AreaPrelievo.ResponseNew.Coda.AreaDati2021 != null && AreaPrelievo.ResponseNew.Coda.AreaDati2021.T_GP1AJTIPCUM != null)
            {
                risposta.GP1AJTIPCUM = AreaPrelievo.ResponseNew.Coda.AreaDati2021.T_GP1AJTIPCUM;
            }
        }

        private static void NormalizzaGP4ToDB(Data.GAIN AreaPrelievo, out RispostaPrelievo risposta)
        {
            risposta = new RispostaPrelievo();

            #region AventiDiritto e Periodi
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaDatiAventiDiritto = null;
            MappingDaHost.ValorizzaAventiDiritto_Periodi(AreaPrelievo, out listaDatiAventiDiritto);
            risposta.ListaDatiAventiDiritto = listaDatiAventiDiritto;
            #endregion AventiDiritto e Periodi
        }

        private static void GestioneRetribuzione(RichiestaPrelievo richiesta, ref List<GestioneCalcolo.DatiCalcoloRetributivo> listaCalcoloRetributivo)
        {
            GestioneControlliDinamici.ControlloDinamico ctrl = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneRetributiviAgoBis", out ctrl);
            if (ctrl != null && ctrl.ValoreControllo == "SI")
            {
                if (listaCalcoloRetributivo != null && listaCalcoloRetributivo.Count > 0 &&
                    listaCalcoloRetributivo.Exists(x => x.NSettimane707.HasValue && x.NSettimane707.Value > 0))
                {
                    GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiGenericiAgoCi = null;
                    string chiavePensione = richiesta.Categoria.ToString().PadLeft(3, '0') + richiesta.Sede.ToString().PadLeft(4, '0') + richiesta.Certificato.ToString().PadLeft(8, '0');
                    List<GestioneCalcolo.DatiCalcoloRetributivo> ldatiRetributiviWS = null;
                    List<GestioneCalcolo.DatiCalcoloContributivo> ldatiContributiviWS = null;

                    string siglaCategoria = "";
                    GestioneDecodifica.AGO_CI_GetCategoriaByCategoriaNumerica(richiesta.Categoria.ToString().PadLeft(4, '0'), out siglaCategoria);
                    GestionePensione.DatiPensione datiPensioneTemp = new GestionePensione.DatiPensione();
                    datiPensioneTemp.SiglaCategoria = siglaCategoria.Trim();

                    string errore = string.Empty;
                    GestioneDatiPensioni.GetDatiTGP2ByChiavePensione(long.Parse(richiesta.NumDomanda), chiavePensione, datiPensioneTemp, out ldatiRetributiviWS, out ldatiContributiviWS,
                        ref datiGenericiAgoCi, out errore);

                    if (!String.IsNullOrEmpty(errore))
                        return;

                    if (ldatiRetributiviWS != null && ldatiRetributiviWS.Count > 0)
                    {
                        foreach (var retr in listaCalcoloRetributivo)
                        {
                            var retrWS = ldatiRetributiviWS.FirstOrDefault(x => x.NSettimane707 == retr.NSettimane707 &&
                                x.QuotePrimeLiquidate == retr.QuotePrimeLiquidate && x.CodiceGestione == retr.CodiceGestione);
                            if (retrWS != null)
                            {
                                retr.PL_Quotar707 = retrWS.PL_Quotar707;
                            }
                        }
                    }
                }
            }
        }
        #endregion private members

        #region nested class
        [Serializable]
        public class RichiestaPrelievo
        {
            public RichiestaPrelievo(short sede, short categoria, int certificato, short sedeOperatore, short centroOperativoOperatore, TipoDomanda tipoDomanda, string numDomanda, string tipo, string prodotto)
            {
                this._Sede = sede;
                this._Categoria = categoria;
                this._Certificato = certificato;
                this._SedeOperatore = sedeOperatore;
                this._CentroOperativoOperatore = centroOperativoOperatore;
                this._TipoDomanda = tipoDomanda;
                this._NumDomanda = numDomanda;
                this._Tipo = tipo;
                this._Prodotto = prodotto;
            }

            #region public properties
            public short Sede { get { return _Sede; } set { _Sede = value; } }
            public short SedeOperatore { get { return _SedeOperatore; } set { _SedeOperatore = value; } }
            public short CentroOperativoOperatore { get { return _CentroOperativoOperatore; } set { _CentroOperativoOperatore = value; } }
            public short Categoria { get { return _Categoria; } set { _Categoria = value; } }
            public int Certificato { get { return _Certificato; } set { _Certificato = value; } }
            public TipoDomanda TipoDomanda { get { return _TipoDomanda; } set { _TipoDomanda = value; } }
            public TipoRicostituzione TipoRicostituzione { get { return _TipoRicostituzione; } set { _TipoRicostituzione = value; } }
            public string NumDomanda { get { return _NumDomanda; } set { _NumDomanda = value; } }
            public string Tipo { get { return _Tipo; } set { _Tipo = value; } }
            public string Prodotto { get { return _Prodotto; } set { _Prodotto = value; } }
            #endregion public properties

            #region private properties
            private short _Sede;
            private short _SedeOperatore;
            private short _CentroOperativoOperatore;
            private short _Categoria;
            private int _Certificato;
            private TipoDomanda _TipoDomanda;
            private TipoRicostituzione _TipoRicostituzione;
            private string _NumDomanda;
            private string _Tipo;
            private string _Prodotto;
            #endregion private properties
        }

        public class RispostaPrelievo
        {
            #region public properties
            public string CodiceFiscale { get { return _CodiceFiscale; } set { _CodiceFiscale = value; } }
            public System.Nullable<DateTime> DataDecorrenza { get { return _DataDecorrenza; } set { _DataDecorrenza = value; } }
            public GestionePensione.DatiPensione DatiPensione { get { return _DatiPensione; } set { _DatiPensione = value; } }
            public GestioneIstruttoria.DatiIstruttoria DatiIstruttoria { get { return _DatiIstruttoria; } set { _DatiIstruttoria = value; } }
            public GestionePagamento.DatiPagamento DatiPagamento { get { return _DatiPagamento; } set { _DatiPagamento = value; } }
            public List<Entity.DatiFamiliari> ListaFamiliari { get { return _ListaFamiliari; } set { _ListaFamiliari = value; } }
            public MappingDaHost.DatiAnagDanteCausa DatiAnagDanteCausa { get { return _DatiAnagDanteCausa; } set { _DatiAnagDanteCausa = value; } }
            public GestioneDanteCausa.DatiDanteCausa DatiDanteCausa { get { return _DatiDanteCausa; } set { _DatiDanteCausa = value; } }
            public List<GestioneAnagrafica.DatiResidenzaEstero> ListaResidenzeEstere { get { return _ListaResidenzeEstere; } set { _ListaResidenzeEstere = value; } }
            public List<GestioneAnagrafica.DatiStatoCivile> ListaStatiCivili { get { return _ListaStatiCivili; } set { _ListaStatiCivili = value; } }
            public MappingDaHost.DatiDelegato DatiDelegato { get { return _DatiDelegato; } set { _DatiDelegato = value; } }
            public MappingDaHost.DatiTutore DatiTutore { get { return _DatiTutore; } set { _DatiTutore = value; } }
            public List<GestioneCalcolo.DatiCalcoloRetributivo> ListaCalcoloRetributivo { get { return _ListaCalcoloRetributivo; } set { _ListaCalcoloRetributivo = value; } }
            public List<GestioneCalcolo.DatiCalcoloContributivo> ListaCalcoloContributivo { get { return _ListaCalcoloContributivo; } set { _ListaCalcoloContributivo = value; } }
            public GestioneDetrazioniImposta.DatiDetrazioni DatiDetrazioni { get { return _DatiDetrazioni; } set { _DatiDetrazioni = value; } }
            public GestionePensione.DatiSindacato DatiSindacato { get { return _DatiSindacato; } set { _DatiSindacato = value; } }
            public List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> ListaSupplementi { get { return _ListaSupplementi; } set { _ListaSupplementi = value; } }
            public INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase DatiSupplementiBase { get { return _DatiSupplementiBase; } set { _DatiSupplementiBase = value; } }
            public GestioneIntegrazioneArt11.IntegrazioneArt11 DatiIntegrazioneArt11 { get { return _DatiIntegrazioneArt11; } set { _DatiIntegrazioneArt11 = value; } }
            public GestionePensione.DatiEliminazione DatiEliminazione { get { return _DatiEliminazione; } set { _DatiEliminazione = value; } }
            public INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici DatiMaggiorazioniBenefici { get { return _DatiMaggiorazioniBenefici; } set { _DatiMaggiorazioniBenefici = value; } }
            public GestioneDatiGenericiAgoCi.PensioniDatiGenerici DatiPensioniDatiGenerici { get { return _DatiPensioniDatiGenerici; } set { _DatiPensioniDatiGenerici = value; } }
            public List<GestioneAltrePensioni.AltraPensione> ListaBititolarita { get { return _ListaBititolarita; } set { _ListaBititolarita = value; } }
            public List<GestionePensioneInailInabilita.DatiPensioniINAIL> ListaInail { get { return _ListaInail; } set { _ListaInail = value; } }
            public GestionePensioneInailInabilita.DatiInabilita DatiInabilita { get { return _DatiInabilita; } set { _DatiInabilita = value; } }
            public List<GestioneOneri.DatiOneri> ListaDatiOneri { get { return _ListaDatiOneri; } set { _ListaDatiOneri = value; } }
            public List<GestioneBeneficiParticolari.DatiBeneficiParticolari> ListaDatiBeneficiParticolari { get { return _ListaDatiBeneficiParticolari; } set { _ListaDatiBeneficiParticolari = value; } }
            public List<GestioneDanteCausa.DatiRedditoSentenza495_93> ListaDatiRedditiSentenza495_93 { get { return _ListaDatiRedditiSentenza495_93; } set { _ListaDatiRedditiSentenza495_93 = value; } }
            public List<GestioneSentenzaArt4.DatiSentenzaArt4> ListaDatiSentenzaArt4 { get { return _ListaDatiSentenzaArt4; } set { _ListaDatiSentenzaArt4 = value; } }
            public List<GestioneCalcolo.QuotePensione> ListaQuotePensione { get; set; }
            public List<GestioneMiglioramentiContrattuali.DatiQuoteMiglioramentiContrattuali> ListaQuoteMiglioramentiContrattuali { get; set; }
            public List<GestioneCalcolo.TrattenuteQuotePensione> ListaTrattenuteQuotePensione { get; set; }
            public GestioneEnpals.DatiEnpals DatiENPALS { get; set; }
            public GestioneCalcolo.DatiCalcoloRetributivoENPAL CalcoloRetributivoENPALS { get; set; }
            public GestioneCalcolo.DatiCalcoloContributivoENPAL CalcoloContributivoENPALS { get; set; }
            public List<Liquidazione.BLCommon.Entity.DatiSupplementiENPALS> ListaSupplementiENPALS { get; set; }
            public List<Liquidazione.BLCommon.Entity.DatiSuppRecordENPALS> ListaSuppRecordENPALS { get; set; }
            public List<GestioneAventiDiritto.AventeDirittoRecuperato> ListaDatiAventiDiritto { get; set; }
            public GestioneBeneficioVittimeTerrorismo.DatiBeneficioVittimeTerrorismo DatiBeneficioVittimeTerrorismo { get; set; }
            public List<GestioneCalcoloVittimeTerrorismo.DatiCalcoloVittimeTerrorismo> ListaDatiCalcoloVittimeTerrorismo { get; set; }
            public string Cittadinanza { get; set; }
            public short CategoriaPensione { get; set; }
            public short SedePensione { get; set; }
            public int CertificatoPensione { get; set; }
            public bool IsRataEstratta { get; set; }
            public string GP1AF03 { get; set; }
            public int? GP1ALB1 { get; set; }
            public short? GP1AXE3 { get; set; }
            public string GP2BB05 { get; set; }
            public short GP1AV91H { get; set; }
            public bool? IsScadenzaAssegnoConGiorno { get; set; }
            public List<GestioneSentenze.DatiSentenze> ListaDatiSentenze { get; set; }
            public List<Liquidazione.BLCommon.Entity.DatiSupplementiCumulo> ListaSupplementiCumulo { get; set; }
            public short? GP1AZ11F { get; set; }
            public List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> ListaQuotaFondoIntegrativo { get { return _ListaQuotaFondoIntegrativo; } set { _ListaQuotaFondoIntegrativo = value; } }
            public List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> ListaDatiRetributiviINPGI { get { return _ListaDatiRetributiviINPGI; } set { _ListaDatiRetributiviINPGI = value; } }
            public List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> ListaDatiContributiviINPGI { get { return _ListaDatiContributiviINPGI; } set { _ListaDatiContributiviINPGI = value; } }
            public string GP1AJTIPCUM { get; set; }
            public bool IsGP1AC01Valorizzato { get; set; }
            public List<GestioneContrib.StatoEsteroCumulo> ListaStatiEsteri { get { return _ListaStatiEsteri; } set { _ListaStatiEsteri = value; } }
            #endregion public properties

            #region private properties
            private string _CodiceFiscale;
            private System.Nullable<DateTime> _DataDecorrenza;
            private GestionePensione.DatiPensione _DatiPensione;
            private GestioneIstruttoria.DatiIstruttoria _DatiIstruttoria;
            private GestionePagamento.DatiPagamento _DatiPagamento;
            private List<Entity.DatiFamiliari> _ListaFamiliari;
            private MappingDaHost.DatiAnagDanteCausa _DatiAnagDanteCausa;
            private GestioneDanteCausa.DatiDanteCausa _DatiDanteCausa;
            private List<GestioneAnagrafica.DatiResidenzaEstero> _ListaResidenzeEstere;
            private List<GestioneAnagrafica.DatiStatoCivile> _ListaStatiCivili;
            private MappingDaHost.DatiDelegato _DatiDelegato;
            private MappingDaHost.DatiTutore _DatiTutore;
            private List<GestioneCalcolo.DatiCalcoloRetributivo> _ListaCalcoloRetributivo;
            private List<GestioneCalcolo.DatiCalcoloContributivo> _ListaCalcoloContributivo;
            private GestioneDetrazioniImposta.DatiDetrazioni _DatiDetrazioni;
            private GestionePensione.DatiSindacato _DatiSindacato;
            private List<INPS.Pensioni.Liquidazione.BLCommon.Entity.DatiSupplementi> _ListaSupplementi;
            private INPS.Pensioni.Liquidazione.BLCommon.Entity.SupplementiBase _DatiSupplementiBase;
            private GestioneIntegrazioneArt11.IntegrazioneArt11 _DatiIntegrazioneArt11;
            private GestionePensione.DatiEliminazione _DatiEliminazione;
            private INPS.Pensioni.Liquidazione.BLCommon.GestioneMaggiorazioniBenefici.DatiMaggiorazioniBenefici _DatiMaggiorazioniBenefici;
            private GestioneDatiGenericiAgoCi.PensioniDatiGenerici _DatiPensioniDatiGenerici;
            private List<GestioneAltrePensioni.AltraPensione> _ListaBititolarita;
            private List<GestionePensioneInailInabilita.DatiPensioniINAIL> _ListaInail;
            private GestionePensioneInailInabilita.DatiInabilita _DatiInabilita;
            private List<GestioneOneri.DatiOneri> _ListaDatiOneri;
            private List<GestioneBeneficiParticolari.DatiBeneficiParticolari> _ListaDatiBeneficiParticolari;
            private List<GestioneDanteCausa.DatiRedditoSentenza495_93> _ListaDatiRedditiSentenza495_93;
            private List<GestioneSentenzaArt4.DatiSentenzaArt4> _ListaDatiSentenzaArt4;
            private List<GestioneQuotaFondoIntegrativo.DatiQuotaFondoIntegrativo> _ListaQuotaFondoIntegrativo;
            private List<GestioneQuotaFondoINPGI.DatiCalcoloRetributivoINPGI> _ListaDatiRetributiviINPGI;
            private List<GestioneQuotaFondoINPGI.DatiCalcoloContributivoINPGI> _ListaDatiContributiviINPGI;
            private MappingDaHostNew.DatiAnagDanteCausa _DatiAnagDanteCausaNew;
            private MappingDaHostNew.DatiDelegato _DatiDelegatoNew;
            private MappingDaHostNew.DatiTutore _DatiTutoreNew;
            private List<GestioneContrib.StatoEsteroCumulo> _ListaStatiEsteri;
            #endregion private properties
        }

        [Serializable]
        public enum TipoDomanda
        {
            Superstiti,
            Ricostituzione,
            Ripristino,
            RipristinoSuperstiti,
            Riliquidazione,
            RiliquidazioneSuperstiti
        };

        [Serializable]
        public enum TipoRicostituzione
        {
            Altro,
            MotiviContributivi,
        }
        #endregion nested class
    }
}




