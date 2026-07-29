using System;
using System.Linq;
using INPS.DNA;

using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterElaborazionePosizione
    {
        public void RicercaDomanda(IView.IRicercaPosizione elaborazionePosizione)
        {
            string sErrore;
            AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
            AreaRispostaRiepilogo risposta = new AreaRispostaRiepilogo();
            AreaEsito esito = new AreaEsito();

            try
            {
                switch (elaborazionePosizione.RicercaPosizione.Selezione)
                {
                    case Utility.TipoRicerca.NDomus://controlli sul numero domanda
                        if (Utility.CheckNDomus(elaborazionePosizione.RicercaPosizione.Domanda, out sErrore))
                        {
                            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.NumeroDomanda;
                            richiesta.NumeroDomanda = elaborazionePosizione.RicercaPosizione.Domanda;
                            if (!string.IsNullOrEmpty(elaborazionePosizione.RicercaPosizione.ProgStorico))
                            {
                                byte progStorico = 0;
                                byte.TryParse(elaborazionePosizione.RicercaPosizione.ProgStorico, out progStorico);
                                richiesta.ProgStorico = progStorico;
                            }
                            richiesta.IsPaginaConferma = elaborazionePosizione.IsPaginaConferma;
                            richiesta.IsConsultazione = elaborazionePosizione.IsConsultazione;
                            if (elaborazionePosizione.RicercaDanteCausa != null)
                            {
                                richiesta.DatiParzialiDanteCausa = new DatiPersonaliParziali();
                                switch (elaborazionePosizione.RicercaDanteCausa.Selezione)
                                {
                                    case Utility.TipoRicerca.NDomusConRicercaCodiceFiscaleDA:
                                        if (Utility.CheckCodiceFiscale(elaborazionePosizione.RicercaDanteCausa.CodiceFiscale.Trim(), out sErrore))
                                        {
                                            richiesta.DatiParzialiDanteCausa.CodiceFiscale = elaborazionePosizione.RicercaDanteCausa.CodiceFiscale;
                                        }
                                        else
                                        {
                                            elaborazionePosizione.ErrorMessage = sErrore;
                                            elaborazionePosizione.HasError = true;
                                            return;
                                        }
                                        break;
                                    case Utility.TipoRicerca.NDomusConRicercaDatiParzialiDA:
                                        richiesta.DatiParzialiDanteCausa.Cognome = elaborazionePosizione.RicercaDanteCausa.Cognome;
                                        richiesta.DatiParzialiDanteCausa.Nome = elaborazionePosizione.RicercaDanteCausa.Nome;
                                        if (!String.IsNullOrEmpty(elaborazionePosizione.RicercaDanteCausa.DataNascita))
                                        {
                                            try
                                            {
                                                DateTime DataNascita = Utility.ConvertString2Data_withMinValue(elaborazionePosizione.RicercaDanteCausa.DataNascita);
                                                richiesta.DatiParzialiDanteCausa.DataNascita = DataNascita;
                                            }
                                            catch (DnaExceptionBase)
                                            {
                                                throw;
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new INPS.DNA.DnaApplicationException("PresenterElaborazionePosizione, Errore nel metodo RicercaDomanda" + ex);
                                            }
                                        }
                                        else
                                            richiesta.DatiParzialiDanteCausa.DataNascita = null;
                                        break;
                                }
                            }
                            GetRiepilogo(richiesta, risposta, elaborazionePosizione);                 
                            return;
                        }
                        else
                        {
                            elaborazionePosizione.ErrorMessage = sErrore;
                            elaborazionePosizione.HasError = true;
                        }
                        break;
                    case Utility.TipoRicerca.CodiceFiscale:     //controlli sul Codice Fiscale
                        if (Utility.CheckCodiceFiscale(elaborazionePosizione.RicercaPosizione.CodiceFiscale.Trim(), out sErrore))
                        {
                            richiesta.TipoRecupero = AreaRichiestaRiepilogo.TipoRicerca.CodiceFiscale;
                            richiesta.CodiceFiscale = elaborazionePosizione.RicercaPosizione.CodiceFiscale;
                            GetRiepilogo(richiesta, risposta, elaborazionePosizione);                           
                            return;
                        }
                        else
                        {
                            elaborazionePosizione.ErrorMessage = sErrore;
                            elaborazionePosizione.HasError = true;
                            return;
                        }
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
                                    throw new INPS.DNA.DnaApplicationException("PresenterElaborazionePosizione, Errore nel metodo RicercaDomanda" + ex);
                                }
                            }
                            else
                                richiesta.DatiParziali.DataNascita = null;
                            GetRiepilogo(richiesta, risposta, elaborazionePosizione);                         
                            return;
                        }
                        else
                        {
                            //stringa errore valorizzata con il messaggio di errore  
                            elaborazionePosizione.ErrorMessage = sErrore;
                            elaborazionePosizione.HasError = true;

                            return;
                        }
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DnaApplicationException("PresenterElaborazionePosizione, Errore nel metodo RicercaDomanda" + ex);
            }
            elaborazionePosizione.HasError = false;
            elaborazionePosizione.ErrorMessage = string.Empty;
            return;
        }

        internal bool GetRiepilogo(AreaRichiestaRiepilogo richiesta, AreaRispostaRiepilogo risposta, IView.IRicercaPosizione elaborazionePosizione)
        {
            bool elencoDomandeIsNull;
            bool elencoPensioniIsNull;
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                richiesta.SedeOperatore = Utility.GetSedeOperatore();
                richiesta.CentroOperativoOperatore = Utility.GetCentroOperativoOperatore();
                richiesta.MatricolaOperatore = Utility.GetMatricolaOperatore();
                richiesta.TipoAppRuolo = elaborazionePosizione.TipoAppRuolo;
                richiesta.Ruolo = elaborazionePosizione.Ruolo;
                richiesta.IsPaginaVisualizzazioneStatoPratiche = elaborazionePosizione.IsPaginaVisualizzazioneStatoPratiche;
                risposta = objWS.GetRiepilogoByKey(richiesta);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterElaborazionePosizione, Errore nel metodo GetRiepilogo");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }

            try
            {
                elaborazionePosizione.IsRicercaManualeDA = risposta.IsRicercaManualeDA;
                elaborazionePosizione.IsNuovoCertificatoGeneratoEnpals = risposta.IsNuovoCertificatoGeneratoEnpals;
                //ENG - Pensioni Ovunque: gestione nuovo pannello
                elaborazionePosizione.MostraPanelloMessBloccantePensioniOvunque = risposta.MostraPanelloMessBloccantePensioniOvunque;
                elaborazionePosizione.SedePensioneGP1ALZ6 = risposta.SedePensioneGP1ALZ6;
                elaborazionePosizione.CodCategoriaPensione = risposta.CodCategoriaPensione;
                elaborazionePosizione.CertificatoInseguimentoPensione = risposta.CertificatoInseguimentoPensione;
                //ENG - Gestione Popup Memo 239
                elaborazionePosizione.MostraPopupMemo239 = risposta.MostraPopupMemo239;
                //ENG - Gestione Popup Memo 31/2023
                elaborazionePosizione.MostraPopupMemo312023 = risposta.MostraPopupMemo312023;

                if (risposta.Esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)  //errore nella risposta del WS
                {
                    elaborazionePosizione.HasError = true;
                    elaborazionePosizione.ErrorMessage = "Errore tecnico nel recupero delle informazioni riguardanti la richiesta effettuata: " + risposta.Esito.Messaggio;
                    return true;
                }
                else if (!String.IsNullOrEmpty(risposta.Esito.Messaggio))
                {
                    elaborazionePosizione.HasError = true;
                    elaborazionePosizione.ErrorMessage = risposta.Esito.Messaggio;
                    elaborazionePosizione.SedeDiversa = risposta.SedeDiversa;
                    if ((risposta.ElencoSinonimi == null || risposta.AnagraficaTitolare != null) && //gestione omonimi
                        !string.IsNullOrEmpty(richiesta.CodiceFiscale) &&
                            risposta.ElencoDomande != null && risposta.ElencoDomande.Length > 0)
                        elaborazionePosizione.HasError = false;
                    else
                        return true;
                }

                //risposta del WS OK o con sola presenza messaggioVideo
                if (risposta.ElencoSinonimi != null && risposta.AnagraficaTitolare == null) //gestione omonimi
                {
                    elaborazionePosizione.ElencoSinonimi = risposta.ElencoSinonimi.ToList();
                }
                else
                {
                    if (risposta.AnagraficaTitolare != null)
                    {
                        elaborazionePosizione.RiepilogoAnagrafica = risposta.AnagraficaTitolare;
                    }

                    if (risposta.ElencoDomande != null)
                    {
                        elaborazionePosizione.ElencoDomande = risposta.ElencoDomande.ToList();
                        elencoDomandeIsNull = false;
                    }
                    else
                    {
                        elencoDomandeIsNull = true;
                    }
                    if (risposta.ElencoPensioni != null)
                    {
                        elaborazionePosizione.ElencoPensioni = risposta.ElencoPensioni.ToList();
                        elencoPensioniIsNull = false;
                    }
                    else
                    {
                        elencoPensioniIsNull = true;
                    }

                    if (elencoDomandeIsNull && elencoPensioniIsNull)
                    { //Elenco domande e pensioni vuote
                        elaborazionePosizione.HasError = true;
                        elaborazionePosizione.ErrorMessage = "Non ci sono posizioni per la chiave di ricerca inserita";
                        return true;
                    }

                    if (risposta.EsitoCalcolo != null)
                    {
                        elaborazionePosizione.EsitoCalcolo = risposta.EsitoCalcolo;
                    }

                    if (String.IsNullOrEmpty(elaborazionePosizione.ErrorMessage) && !String.IsNullOrEmpty(risposta.Esito.MsgNonBloccante))
                    {
                        elaborazionePosizione.ErrorMessage = risposta.Esito.MsgNonBloccante;
                    }

                    elaborazionePosizione.IsDomandaDB = risposta.IsDomandaDB;
                    elaborazionePosizione.IsDomandaCalcolataProvvisoria = risposta.IsDomandaCalcolataProvvisoria;
                }
            }
            catch (DnaExceptionBase)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new INPS.DNA.DnaApplicationException("PresenterElaborazionePosizione, Errore nel metodo GetRiepilogo" + ex);
            }
            return false;
        }
    }
}
