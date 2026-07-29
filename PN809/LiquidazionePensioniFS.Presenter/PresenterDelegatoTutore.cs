using System;
using System.ServiceModel;
using System.Linq;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDelegatoTutore
    {
        public void RicercaDelegato(IView.IRicercaPosizione elaborazionePosizione)
        {
            string sErrore;
            AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();

            AreaEsito esito = new AreaEsito();

            richiesta.NumeroDomanda = elaborazionePosizione.RicercaPosizione.Domanda;

            try
            {
                switch (elaborazionePosizione.RicercaPosizione.Selezione)
                {
                    case Utility.TipoRicerca.CodiceFiscale:     //controlli sul Codice Fiscale
                        if (Utility.CheckCodiceFiscale(elaborazionePosizione.RicercaPosizione.CodiceFiscale.Trim(), out sErrore))
                        {
                            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
                            richiesta.CodiceFiscale = elaborazionePosizione.RicercaPosizione.CodiceFiscale;
                            richiesta.NumeroDomanda = elaborazionePosizione.RicercaPosizione.Domanda;
                            GetRiepilogoCF(richiesta, risposta, elaborazionePosizione);
                            if (risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            {
                                elaborazionePosizione.ErrorMessage = risposta.Esito.Messaggio;
                                elaborazionePosizione.HasError = true;
                                return;
                            }
                            else if (risposta.AnagraficaTitolare == null && risposta.ElencoSinonimi == null)
                            {
                                elaborazionePosizione.ErrorMessage = "Nessun soggetto trovato per i parametri inseriti";
                                elaborazionePosizione.HasError = true;
                                return;
                            }
                            else
                            {
                                elaborazionePosizione.RiepilogoAnagrafica = risposta.AnagraficaTitolare;
                            }
                        }
                        else
                        {
                            elaborazionePosizione.ErrorMessage = "Dati inseriti errati ";
                            elaborazionePosizione.HasError = true;
                            return;
                        }
                        break;
                    case Utility.TipoRicerca.Anagrafica:
                        if (Utility.CheckAnagrafica(elaborazionePosizione.RicercaPosizione.Nome, elaborazionePosizione.RicercaPosizione.Cognome, elaborazionePosizione.RicercaPosizione.DataNascita, out sErrore))
                        {
                            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
                            richiesta.DatiParziali = new DatiPersonaliParziali();
                            richiesta.DatiParziali.Cognome = elaborazionePosizione.RicercaPosizione.Cognome;
                            richiesta.DatiParziali.Nome = elaborazionePosizione.RicercaPosizione.Nome;
                            if (!String.IsNullOrEmpty(elaborazionePosizione.RicercaPosizione.DataNascita))
                            {
                                try
                                {
                                    DateTime DataNascita = Utility.ConvertString2Data_withMinValue(elaborazionePosizione.RicercaPosizione.DataNascita);
                                    richiesta.DatiParziali.DataNascita = DataNascita;
                                }
                                catch (DnaExceptionBase)
                                {
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    throw new INPS.DNA.DnaApplicationException("PresenterDelegatoTutore, Errore nel metodo RicercaDomanda" + ex);
                                }
                            }
                            else
                                richiesta.DatiParziali.DataNascita = null;
                            risposta = GetRiepilogoAnagrafica(richiesta, risposta, elaborazionePosizione);
                            if (risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            {
                                elaborazionePosizione.ErrorMessage = risposta.Esito.Messaggio;
                                elaborazionePosizione.HasError = true;
                                return;
                            }
                            else if (risposta.AnagraficaTitolare == null && risposta.ElencoSinonimi == null)
                            {
                                elaborazionePosizione.ErrorMessage = "Nessun soggetto trovato per i parametri inseriti";
                                elaborazionePosizione.HasError = true;
                                return;
                            }
                            else if (risposta.AnagraficaTitolare == null && risposta.ElencoSinonimi != null)
                            {
                                elaborazionePosizione.ElencoSinonimi = risposta.ElencoSinonimi.ToList();
                            }
                            else
                            {
                                elaborazionePosizione.RiepilogoAnagrafica = risposta.AnagraficaTitolare;
                            }
                        }
                        else
                        {
                            elaborazionePosizione.ErrorMessage = "Dati inseriti errati ";
                            elaborazionePosizione.HasError = true;
                            return;
                        }
                        elaborazionePosizione.RiepilogoAnagrafica = risposta.AnagraficaTitolare;
                        break;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterDelegatoTutore, Errore nel metodo RicercaDomanda" + ex);
            }
            elaborazionePosizione.HasError = false;
            elaborazionePosizione.ErrorMessage = string.Empty;

            return;
        }

        public void RicercaTutore(IView.IRicercaPosizione elaborazionePosizione)
        {
            string sErrore;
            Presenter.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            Presenter.SvrLiquidazione.AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();

            Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
            richiesta.NumeroDomanda = elaborazionePosizione.RicercaPosizione.Domanda;

            try
            {
                switch (elaborazionePosizione.RicercaPosizione.Selezione)
                {

                    case Utility.TipoRicerca.CodiceFiscale:     //controlli sul Codice Fiscale
                        if (Utility.CheckCodiceFiscale(elaborazionePosizione.RicercaPosizione.CodiceFiscale.Trim(), out sErrore))
                        {
                            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
                            richiesta.CodiceFiscale = elaborazionePosizione.RicercaPosizione.CodiceFiscale;
                            GetRiepilogoCFTutore(richiesta, risposta, elaborazionePosizione);
                            if (risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            {
                                elaborazionePosizione.ErrorMessage = risposta.Esito.Messaggio;
                                elaborazionePosizione.HasError = true;
                                return;
                            }
                            else if (risposta.AnagraficaTitolare == null)
                            {
                                elaborazionePosizione.ErrorMessage = "Nessun soggetto trovato per i parametri inseriti";
                                elaborazionePosizione.HasError = true;
                                return;
                            }

                            elaborazionePosizione.RiepilogoAnagrafica = risposta.AnagraficaTitolare;
                        }
                        else
                        {
                            elaborazionePosizione.ErrorMessage = "Codice Fiscale non valido";
                            elaborazionePosizione.HasError = true;
                            return;
                        }
                        break;
                    case Utility.TipoRicerca.Anagrafica:
                        if (Utility.CheckAnagrafica(elaborazionePosizione.RicercaPosizione.Nome, elaborazionePosizione.RicercaPosizione.Cognome, elaborazionePosizione.RicercaPosizione.DataNascita, out sErrore))
                        {
                            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.DatiPersonaliParziali;
                            richiesta.DatiParziali = new DatiPersonaliParziali();
                            richiesta.DatiParziali.Cognome = elaborazionePosizione.RicercaPosizione.Cognome;
                            richiesta.DatiParziali.Nome = elaborazionePosizione.RicercaPosizione.Nome;
                            if (!String.IsNullOrEmpty(elaborazionePosizione.RicercaPosizione.DataNascita))
                            {
                                try
                                {
                                    DateTime DataNascita = Utility.ConvertString2Data_withMinValue(elaborazionePosizione.RicercaPosizione.DataNascita);
                                    richiesta.DatiParziali.DataNascita = DataNascita;
                                }
                                catch (DnaExceptionBase)
                                {
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    throw new INPS.DNA.DnaApplicationException("PresenterDelegatoTutore, Errore nel metodo RicercaDomanda" + ex);
                                }
                            }
                            else
                                richiesta.DatiParziali.DataNascita = null;
                            risposta = GetRiepilogoAnagrafica(richiesta, risposta, elaborazionePosizione);
                            if (risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                            {
                                elaborazionePosizione.ErrorMessage = risposta.Esito.Messaggio;
                                elaborazionePosizione.HasError = true;
                                return;
                            }
                            else if (risposta.AnagraficaTitolare == null && risposta.ElencoSinonimi == null)
                            {
                                elaborazionePosizione.ErrorMessage = "Nessun soggetto trovato per i parametri inseriti";
                                elaborazionePosizione.HasError = true;
                                return;
                            }
                            else if (risposta.AnagraficaTitolare == null && risposta.ElencoSinonimi != null)
                            {
                                elaborazionePosizione.ElencoSinonimi = risposta.ElencoSinonimi.ToList();
                            }
                            else
                            {
                                elaborazionePosizione.RiepilogoAnagrafica = risposta.AnagraficaTitolare;
                            }
                        }
                        else
                        {
                            elaborazionePosizione.ErrorMessage = "Dati inseriti errati ";
                            elaborazionePosizione.HasError = true;
                            return;
                        }
                        elaborazionePosizione.RiepilogoAnagrafica = risposta.AnagraficaTitolare;
                        break;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterDelegatoTutore, Errore nel metodo RicercaDomanda" + ex);
            }
            elaborazionePosizione.HasError = false;
            elaborazionePosizione.ErrorMessage = string.Empty;

            return;
        }

        internal bool GetRiepilogoCF(AreaRichiestaRiepilogo richiesta, AreaRispostaRiepilogo risposta, IView.IRicercaPosizione elaborazionePosizione)
        {
            Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica risposta2 = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            try
            {
                esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out risposta2, richiesta.CodiceFiscale, Utility.GetSedeOperatore(), Utility.GetMatricolaOperatore(), richiesta.NumeroDomanda);
                risposta.Esito = esito;
                risposta.AnagraficaTitolare = risposta2;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo GetRiepilogoCF");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return false;
        }

        internal AreaRispostaRiepilogo GetRiepilogoAnagrafica(AreaRichiestaRiepilogo richiesta, AreaRispostaRiepilogo risposta, IView.IRicercaPosizione elaborazionePosizione)
        {
            Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica risposta2 = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            try
            {
                risposta = objWS.GetAnagraficaByDatiPersonaliParziali(Utility.GetSedeOperatore(), Utility.GetMatricolaOperatore(), richiesta.DatiParziali, richiesta.NumeroDomanda);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo GetRiepilogoAnagrafica");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return risposta;
        }

        internal bool GetRiepilogoCFTutore(AreaRichiestaRiepilogo richiesta, AreaRispostaRiepilogo risposta, IView.IRicercaPosizione elaborazionePosizione)
        {
            Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica risposta2 = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            try
            {
                esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out risposta2, richiesta.CodiceFiscale, Utility.GetSedeOperatore(), Utility.GetMatricolaOperatore(), richiesta.NumeroDomanda);
                risposta.Esito = esito;
                risposta.AnagraficaTitolare = risposta2;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo GetRiepilogoCFTutore");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return false;
        }

        public AreaRispostaRiepilogo.DatiRiepilogoAnagrafica CaricaDelegato(IDelegatoTutore DelegatoTutore)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica delegato2 = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            try
            {
                AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(DelegatoTutore.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = DelegatoTutore.domanda.ProgStorico;

                esito = objWS.GetDelegatoByNumeroDomanda(out delegato2, areaRichiestaDomanda);
                DelegatoTutore.delegato.AnagraficaTitolare = delegato2;
                return DelegatoTutore.delegato.AnagraficaTitolare;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                DelegatoTutore.HasError = true;
                DelegatoTutore.ErrorMessage = string.Format("Errore CaricaDelegato: {0}", Ex.Message);
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
            return delegato2;
        }

        public AreaRispostaRiepilogo.DatiRiepilogoAnagrafica CaricaTutore(IDelegatoTutore DelegatoTutore)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            AreaRispostaRiepilogo.DatiRiepilogoAnagrafica tutore2 = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
            try
            {
                AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(DelegatoTutore.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = DelegatoTutore.domanda.ProgStorico;

                esito = objWS.GetTutoreByNumeroDomanda(out tutore2, areaRichiestaDomanda);
                DelegatoTutore.tutore.AnagraficaTitolare = tutore2;
                return DelegatoTutore.tutore.AnagraficaTitolare;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
            {
                throw;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract>)
            {
                throw;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                throw;
            }
            catch (Exception Ex)
            {
                DelegatoTutore.HasError = true;
                DelegatoTutore.ErrorMessage = string.Format("Errore CaricaDelegato: {0}", Ex.Message);
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
            return tutore2;
        }

        public void SalvaDatiDelegato(IDelegatoTutore DelegatoTutore)
        {
            try
            {
                string sErrore;
                SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                SvrLiquidazione.AreaEsito esito = new SvrLiquidazione.AreaEsito();

                if ((Utility.CheckNTelefono(DelegatoTutore.delegato.AnagraficaTitolare.Tel, out sErrore)) &&
                    (Utility.CheckNTelefono(DelegatoTutore.delegato.AnagraficaTitolare.Cell, out sErrore)) &&
                    (Utility.CheckEmail(DelegatoTutore.delegato.AnagraficaTitolare.EMail, out sErrore)))
                {
                    if (!String.IsNullOrEmpty(DelegatoTutore.delegato.AnagraficaTitolare.CodiceFiscale))
                    {
                        if (DelegatoTutore.delegato.AnagraficaTitolare.CodiceDelegato != null)
                        {
                            try
                            {
                                esito = objWS.StoreDelegato(Int64.Parse(DelegatoTutore.domanda.NumeroDomanda), DelegatoTutore.delegato.AnagraficaTitolare);
                                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                                {
                                    sErrore = esito.Messaggio;
                                    DelegatoTutore.HasError = true;
                                    DelegatoTutore.ErrorMessage = esito.Messaggio;
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo SalvaDatiDelegato");
                            }
                            finally
                            {
                                Utility.CloseClient(objWS);
                            }
                        }
                        else
                        {
                            sErrore = "Impossibile salvare prima di aver scelto il codice Delegato.";
                            DelegatoTutore.ErrorMessage = sErrore;
                            DelegatoTutore.HasError = true;
                        }
                    }
                    else
                    {
                        sErrore = "Impossibile salvare prima di aver effettuato la ricerca.";
                        DelegatoTutore.ErrorMessage = sErrore;
                        DelegatoTutore.HasError = true;
                    }
                }
                else
                {
                    DelegatoTutore.ErrorMessage = sErrore;
                    DelegatoTutore.HasError = true;
                }
                return;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterDelegato, Errore nel metodo SalvaDatiDelegato" + ex);
            }
        }

        public void SalvaDatiTutore(IDelegatoTutore DelegatoTutore)
        {
            try
            {
                string sErrore;
                SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
                SvrLiquidazione.AreaEsito esito = new SvrLiquidazione.AreaEsito();

                if ((Utility.CheckNTelefono(DelegatoTutore.tutore.AnagraficaTitolare.Tel, out sErrore)) &&
                        (Utility.CheckNTelefono(DelegatoTutore.tutore.AnagraficaTitolare.Cell, out sErrore)) &&
                        (Utility.CheckEmail(DelegatoTutore.tutore.AnagraficaTitolare.EMail, out sErrore)))
                {
                    if (!String.IsNullOrEmpty(DelegatoTutore.tutore.AnagraficaTitolare.CodiceFiscale))
                    {
                        if (DelegatoTutore.tutore.AnagraficaTitolare.CodiceTutore != null)
                        {
                            try
                            {
                                esito = objWS.StoreTutore(Int64.Parse(DelegatoTutore.domanda.NumeroDomanda), DelegatoTutore.tutore.AnagraficaTitolare);
                                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                                {
                                    sErrore = esito.Messaggio;
                                    DelegatoTutore.HasError = true;
                                    DelegatoTutore.ErrorMessage = esito.Messaggio;
                                }
                            }
                            catch (Exception ex)
                            {
                                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo SalvaDatiTutore");
                            }
                            finally
                            {
                                Utility.CloseClient(objWS);
                            }
                        }
                        else
                        {
                            sErrore = "Impossibile salvare prima di aver scelto il codice Tutore.";
                            DelegatoTutore.ErrorMessage = sErrore;
                            DelegatoTutore.HasError = true;
                        }
                    }
                    else
                    {
                        sErrore = "Impossibile salvare prima di aver effettuato la ricerca.";
                        DelegatoTutore.ErrorMessage = sErrore;
                        DelegatoTutore.HasError = true;
                    }
                }
                else
                {
                    DelegatoTutore.ErrorMessage = sErrore;
                    DelegatoTutore.HasError = true;
                }
                return;
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterDelegato, Errore nel metodo SalvaDatiDelegato" + ex);
            }
        }

        public void SalvaDelegatoTutore(IDelegatoTutore DelegatoTutore)
        {

            string sErrore = string.Empty;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            SvrLiquidazione.AreaEsito esito = new SvrLiquidazione.AreaEsito();
            bool bCheckDelegato = true;
            bool bCheckTutore = true;
            try
            {
                if (DelegatoTutore.delegato.AnagraficaTitolare != null)
                    if (!((Utility.CheckNTelefono(DelegatoTutore.delegato.AnagraficaTitolare.Tel, out sErrore)) && (Utility.CheckNTelefono(DelegatoTutore.delegato.AnagraficaTitolare.Cell, out sErrore)) && (Utility.CheckEmail(DelegatoTutore.delegato.AnagraficaTitolare.EMail, out sErrore))))
                        bCheckDelegato = false;
                if (DelegatoTutore.tutore.AnagraficaTitolare != null)
                    if (!((Utility.CheckNTelefono(DelegatoTutore.tutore.AnagraficaTitolare.Tel, out sErrore)) && (Utility.CheckNTelefono(DelegatoTutore.tutore.AnagraficaTitolare.Cell, out sErrore)) && (Utility.CheckEmail(DelegatoTutore.tutore.AnagraficaTitolare.EMail, out sErrore))))
                        bCheckTutore = false;

                //if ((DelegatoTutore.delegato.AnagraficaTitolare == null || ((Utility.CheckNTelefono(DelegatoTutore.delegato.AnagraficaTitolare.Tel, out  sErrore)) && (Utility.CheckNTelefono(DelegatoTutore.delegato.AnagraficaTitolare.Cell, out sErrore)) && (Utility.CheckEmail(DelegatoTutore.delegato.AnagraficaTitolare.EMail, out sErrore)))) ||
                //    (DelegatoTutore.tutore.AnagraficaTitolare   == null || ((Utility.CheckNTelefono(DelegatoTutore.tutore.AnagraficaTitolare.Tel, out  sErrore)) && (Utility.CheckNTelefono(DelegatoTutore.tutore.AnagraficaTitolare.Cell, out sErrore)) && (Utility.CheckEmail(DelegatoTutore.tutore.AnagraficaTitolare.EMail, out sErrore)))))
                if (bCheckDelegato && bCheckTutore)
                {
                    esito = objWS.StoreDelegatoTutore(Int64.Parse(DelegatoTutore.domanda.NumeroDomanda), DelegatoTutore.delegato.AnagraficaTitolare, DelegatoTutore.tutore.AnagraficaTitolare);
                    if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                    {
                        sErrore = esito.Messaggio;
                        DelegatoTutore.HasError = true;
                        DelegatoTutore.ErrorMessage = esito.Messaggio;
                    }
                }
                else
                {
                    DelegatoTutore.ErrorMessage = sErrore;
                    DelegatoTutore.HasError = true;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo SalvaDelegatoTutore");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return;
        }

        public void IsNotDelegatoPresent(IDelegatoTutore DelegatoTutore)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(DelegatoTutore.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = DelegatoTutore.domanda.ProgStorico;
            try
            {
                esito = objWS.IsNotDelegatoOrTutorePresent(areaRichiestaDomanda, true);
                if (esito.RisultatoOperazione == SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    DelegatoTutore.HasError = true;
                    DelegatoTutore.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo IsNotDelegatoPresent");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return;
        }

        public void IsNotTutorePresent(IDelegatoTutore DelegatoTutore)
        {
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(DelegatoTutore.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = DelegatoTutore.domanda.ProgStorico;

            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            AreaEsito esito = new AreaEsito();
            try
            {
                esito = objWS.IsNotDelegatoOrTutorePresent(areaRichiestaDomanda, false);
                if (esito.RisultatoOperazione == SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    DelegatoTutore.HasError = true;
                    DelegatoTutore.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo IsNotTutorePresent");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            return;
        }

        public void EliminaDelegato(IDelegatoTutore DelegatoTutore)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            Int64 ndomanda = Int64.Parse(DelegatoTutore.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();
            try
            {
                esito = objWS.DeleteDelegato(ndomanda);

            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo EliminaDelegato");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                DelegatoTutore.HasError = true;
                DelegatoTutore.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                DelegatoTutore.HasError = false;
                DelegatoTutore.ErrorMessage = "";
                DelegatoTutore.delegato = new AreaRispostaRiepilogo();
                return;
            }
        }

        public void EliminaTutore(IDelegatoTutore DelegatoTutore)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            Int64 ndomanda = Int64.Parse(DelegatoTutore.domanda.NumeroDomanda);
            AreaEsito esito = new AreaEsito();
            try
            {
                esito = objWS.DeleteTutore(ndomanda);

            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDelegatoTutore, Errore nel metodo EliminaTutore");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
            {
                DelegatoTutore.HasError = true;
                DelegatoTutore.ErrorMessage = esito.Messaggio;
                return;
            }
            else
            {
                DelegatoTutore.HasError = false;
                DelegatoTutore.ErrorMessage = "";
                DelegatoTutore.tutore = new AreaRispostaRiepilogo();
                return;
            }
        }
    }
}
