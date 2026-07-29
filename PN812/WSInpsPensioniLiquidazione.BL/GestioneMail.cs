using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.Entity;
using System.ServiceModel;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Configuration;
using INPS.Pensioni.Liquidazione.ServiceReferences.Icona;
using System.ServiceModel.Description;
using System.Collections;
using System.Reflection;
using INPS.Pensioni.Liquidazione.Service_Reference.SrvIcona2;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneMail
    {
        public static bool NotificaSegnalazione(Segnalazione segnalazione, out string errori)
        {
            errori = string.Empty;
            CompletaMessaggioSegnalazione(segnalazione);
            if (!InvioSegnalazioneIcona2(segnalazione, out errori))
                return false;
            
            return true;
        }

        public static bool NotificaSbloccoDomandaUnicarpe(string matricolaOperatore, long nDomus, string codiceFiscale, short sede, string messaggio, string mailTo, out string errori)
        {
            errori = string.Empty;
            if (!InvioNotificaSbloccoDomandaUnicarpeIcona2(matricolaOperatore, nDomus, codiceFiscale, sede, messaggio, mailTo, out errori))
                return false;

            return true;
        }

        public static void GetChiaviIcona(out string code, out string userName, out string password)
        {
            code = ConfigurationManager.AppSettings["CodeIcona"];
            userName = ConfigurationManager.AppSettings["UsrIcona"];
            password = ConfigurationManager.AppSettings["PwdIcona"];
        }

        public static bool ControlsDatiMail(Segnalazione segnalazione, out string errori)
        {
            errori = string.Empty;

            if (segnalazione == null)
            {
                errori = "Dati di segnalazione mancanti";
                return false;
            }

            //switch (segnalazione.Tipologia.ToUpperInvariant())
            //{
            //    case "ERRORI DATI DELLA DOMANDA":
            //    case "ERRORI DATI ANAGRAFICI":
            //        if (string.IsNullOrEmpty(segnalazione.NDomus))
            //        {
            //            errori = "Numero domus mancante";
            //            return false;
            //        }

            //        if (string.IsNullOrEmpty(segnalazione.CodiceFiscale))
            //        {
            //            errori = "Codice fiscale mancante";
            //            return false;
            //        }

            //        if (string.IsNullOrEmpty(segnalazione.Categoria) || string.IsNullOrEmpty(segnalazione.Sede) || string.IsNullOrEmpty(segnalazione.Certificato))
            //        {
            //            errori = "Chiave pensione mancante";
            //            return false;
            //        }
            //        break;
            //    case "ERRORI DEL CALCOLO":
            //        if (string.IsNullOrEmpty(segnalazione.NDomus))
            //        {
            //            errori = "Numero domus mancante";
            //            return false;
            //        }

            //        if (string.IsNullOrEmpty(segnalazione.CodiceFiscale))
            //        {
            //            errori = "Codice fiscale mancante";
            //            return false;
            //        }

            //        if (string.IsNullOrEmpty(segnalazione.Categoria) || string.IsNullOrEmpty(segnalazione.Sede) || string.IsNullOrEmpty(segnalazione.Certificato))
            //        {
            //            errori = "Chiave pensione mancante";
            //            return false;
            //        }

            //        if (string.IsNullOrEmpty(segnalazione.CodiceErrore))
            //        {
            //            errori = "Codice errore mancante";
            //            return false;
            //        }

            //        if (!segnalazione.DecorrenzaPensione.HasValue)
            //        {
            //            errori = "Decorrenza pensione mancante";
            //            return false;
            //        }
            //        break;
            //}

            if (string.IsNullOrEmpty(segnalazione.MatricolaOperatore))
            {
                errori = "Matricola Operatore assente";
                return false;
            }

            if (string.IsNullOrEmpty(segnalazione.SedeOperatore))
            {
                errori = "Sede Operatore assente";
                return false;
            }

            if (string.IsNullOrEmpty(segnalazione.RecapitoMittente))
            {
                errori = "Telefono mancante";
                return false;
            }

            return true;
        }

        private static void CompletaMessaggioSegnalazione(Segnalazione segnalazione)
        {
            List<GestioneVersioni.DatiVersioni> listaVersioni = null;
            GestioneVersioni.GetVersioni(out listaVersioni);

            segnalazione.Messaggio += Environment.NewLine;
            segnalazione.Messaggio += Environment.NewLine;
            segnalazione.Messaggio += "Informazioni aggiuntive:" + Environment.NewLine;
            if (!string.IsNullOrEmpty(segnalazione.MatricolaOperatore))
                segnalazione.Messaggio += string.Concat("Matricola: ", segnalazione.MatricolaOperatore, Environment.NewLine);
            if (!string.IsNullOrEmpty(segnalazione.NomeMittente) && !string.IsNullOrEmpty(segnalazione.CognomeMittente))
                segnalazione.Messaggio += string.Concat("Utente: ", segnalazione.NomeMittente + " " + segnalazione.CognomeMittente, Environment.NewLine);
            if (!string.IsNullOrEmpty(segnalazione.SedeOperatore))
                segnalazione.Messaggio += string.Concat("Sede: ", segnalazione.SedeOperatore, Environment.NewLine);
            if (!string.IsNullOrEmpty(segnalazione.RecapitoMittente))
                segnalazione.Messaggio += string.Concat("Recapito telefonico: ", segnalazione.RecapitoMittente, Environment.NewLine);
            if (!string.IsNullOrEmpty(segnalazione.NDomus))
                segnalazione.Messaggio += string.Concat("Numero domus: ", segnalazione.NDomus, Environment.NewLine);
            if (!string.IsNullOrEmpty(segnalazione.CodiceFiscale))
                segnalazione.Messaggio += string.Concat("Codice fiscale titolare: ", segnalazione.CodiceFiscale, Environment.NewLine);
            if (!string.IsNullOrEmpty(segnalazione.CodiceErrore))
                segnalazione.Messaggio += string.Concat("Codice errore: ", segnalazione.CodiceErrore, Environment.NewLine);

        
            if (!string.IsNullOrEmpty(segnalazione.Categoria) && !string.IsNullOrEmpty(segnalazione.Sede) && !string.IsNullOrEmpty(segnalazione.Certificato))
            {
                segnalazione.Messaggio += Environment.NewLine;
                segnalazione.Messaggio += string.Concat("Dati pensione", Environment.NewLine);
                segnalazione.Messaggio += string.Concat("Categoria: ", segnalazione.Categoria,  Environment.NewLine);
                segnalazione.Messaggio += string.Concat("Sede: ", segnalazione.Sede, Environment.NewLine);
                segnalazione.Messaggio += string.Concat("Certificato: ", segnalazione.Certificato, Environment.NewLine);
            }
            if (segnalazione.DecorrenzaPensione.HasValue)
            {
                segnalazione.Messaggio += string.Concat("Decorrenza: ",segnalazione.DecorrenzaPensione.Value.ToString("dd/MM/yyyy"), Environment.NewLine);
            }

            segnalazione.Messaggio += Environment.NewLine;
            segnalazione.Messaggio += string.Concat("Versioni:", Environment.NewLine);
            segnalazione.Messaggio += string.Concat(Utility.SetVersioni(listaVersioni, null, Utility.ChiaviVersioni.WA), Environment.NewLine);
            segnalazione.Messaggio += string.Concat(Utility.SetVersioni(listaVersioni, null, Utility.ChiaviVersioni.WCF), Environment.NewLine);

            if (segnalazione.tipoApp.HasValue)
                segnalazione.Messaggio += string.Concat(Utility.SetVersioni(listaVersioni, segnalazione.tipoApp, Utility.ChiaviVersioni.WCF), Environment.NewLine);
        }

        private static bool InvioSegnalazione(Segnalazione segnalazione, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            string stackTrace = null;

            string codeIcona = string.Empty;
            string usernameIcona = string.Empty;
            string passwordIcona = string.Empty;

            InvioSoapClient proxy = new InvioSoapClient();

            try
            {
                GetChiaviIcona(out codeIcona, out usernameIcona, out passwordIcona);

                // Dati di input
                InvioMail_INPUT_01 mailinput = new InvioMail_INPUT_01();

                mailinput.mittente = string.Concat(segnalazione.MatricolaOperatore, "@inps.it");
                GestioneControlliDinamici.ControlloDinamico endpointIconaProd = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("EndpointIconaProd", out endpointIconaProd);
                if (endpointIconaProd != null && endpointIconaProd.ValoreControllo == "SI")
                    mailinput.subject = "Sviluppo - ";
                mailinput.subject = mailinput.subject + segnalazione.Procedura;
                mailinput.subject = mailinput.subject + " - Matricola: " + segnalazione.MatricolaOperatore + " - Sede: " + segnalazione.SedeOperatore.Substring(0, 6) + " - ";
                mailinput.subject = mailinput.subject + segnalazione.Tipologia + (!string.IsNullOrEmpty(segnalazione.NDomus) ? " - Domus: " + segnalazione.NDomus : string.Empty);
                mailinput.subject = mailinput.subject + (!string.IsNullOrEmpty(segnalazione.Categoria) ? " - Pensione: " + segnalazione.Categoria : string.Empty);
                mailinput.subject = mailinput.subject + (!string.IsNullOrEmpty(segnalazione.Sede) ? " - " + segnalazione.Sede : string.Empty);
                mailinput.subject = mailinput.subject + (!string.IsNullOrEmpty(segnalazione.Certificato) ? " - " + segnalazione.Certificato : string.Empty);
                mailinput.body = segnalazione.Messaggio;
                mailinput.codiceApplicazione = codeIcona;

                // parametri aggiuntivi non specificati dall'utente
                // chiave univoca
                Guid guid = Guid.NewGuid();
                mailinput.keygest = "LIQPENS-" + guid.ToString();
                //mailinput.sede = segnalazione.Sede.ToString().PadLeft(4, '0');
                mailinput.PEC = false;
                mailinput.checkEsitoInvio = true;

                //Invio e autenticazione
                LoginHeader login = new LoginHeader();
                login.UserName = usernameIcona;
                login.Password = passwordIcona;

                GestioneControlliDinamici.ControlloDinamico abilitaIconaTest = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaIconaTest", out abilitaIconaTest);
                if (abilitaIconaTest != null && abilitaIconaTest.ValoreControllo == "SI")
                {
                    GestioneControlliDinamici.ControlloDinamico elencoMailIcona = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ElencoMailIcona", out elencoMailIcona);
                    if (elencoMailIcona != null && !string.IsNullOrEmpty(elencoMailIcona.ValoreControllo))
                    {
                        string[] elenco = elencoMailIcona.ValoreControllo.Split(';');
                        mailinput.destinatari = new Destinatari_01[elenco.Count()];

                        for (int i = 0; i < elenco.Count(); i++)
                        {
                            mailinput.destinatari[i] = new Destinatari_01();
                            mailinput.destinatari[i].destinatario = elenco[i].Trim();
                            if (i % 2 == 0)
                                mailinput.destinatari[i].tipo = tipoDestinatari_01.to;
                            else
                                mailinput.destinatari[i].tipo = tipoDestinatari_01.bcc;
                        }
                    }
                }
                else
                {
                    mailinput.destinatari = new Liquidazione.ServiceReferences.Icona.Destinatari_01[segnalazione.Destinatari.Count];

                    for (int i = 0; i < segnalazione.Destinatari.Count; i++)
                    {
                        if (segnalazione.Destinatari[i] == mailinput.mittente)
                        {
                            mailinput.destinatari[i] = new Liquidazione.ServiceReferences.Icona.Destinatari_01();
                            mailinput.destinatari[i].destinatario = segnalazione.Destinatari[i];
                            mailinput.destinatari[i].tipo = Liquidazione.ServiceReferences.Icona.tipoDestinatari_01.bcc;
                        }
                        else
                        {
                            mailinput.destinatari[i] = new Liquidazione.ServiceReferences.Icona.Destinatari_01();
                            mailinput.destinatari[i].destinatario = segnalazione.Destinatari[i];
                            mailinput.destinatari[i].tipo = Liquidazione.ServiceReferences.Icona.tipoDestinatari_01.to;
                        }
                    }
                }

                if (endpointIconaProd != null && endpointIconaProd.ValoreControllo == "SI")
                {
                    proxy.Endpoint.Address = new EndpointAddress("http://intranet.inps/Ws.Net/WsIcona/Invio.asmx");
                    login.Password = "aa_reee1£a";
                }

                INPS.Pensioni.Liquidazione.ServiceReferences.Icona.InvioMail_OUTPUT output = proxy.InvioEmail_01(login, mailinput);
                if (output.CD_RC == INPS.Pensioni.Liquidazione.ServiceReferences.Icona.ResultType.ER)
                {
                    errori = output.errorDescription;
                    return false;
                }
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                errori = Utility.GetMessageFromException(exception);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return false;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
            {
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.WriteError(errori);
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio Icona, method InvioEmail: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.WriteError(errori);
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                {
                    string messaggio = errori;
                    errori = "Errore tecnico durante l'invio della segnalazione";
                    string parametri = null;
                    try
                    {
                        parametri = Utility.GetXmlFromObject(segnalazione);
                    }
                    catch(Exception)
                    {
                        // Eccezione ignorata
                    }
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
                Utility.CloseClient(proxy);
            }
            return true;
        }
        private static bool InvioSegnalazioneIcona2(Segnalazione segnalazione, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            string stackTrace = null;

            string codeIcona = string.Empty;
            string usernameIcona = string.Empty;
            string passwordIcona = string.Empty;
            string msgSubject = string.Empty;
            string requestIcona2 = string.Empty;

            try
            {
                WSIcona20Client wsIcona20Client = new WSIcona20Client();
                InvioEmailExtRequest invioEmailExtRequest = new InvioEmailExtRequest();

                // chiave univoca
                Guid guid = Guid.NewGuid();
                invioEmailExtRequest.chiaveGestionale = "LIQPENS-" + guid.ToString();

                //mittente
                invioEmailExtRequest.mittente = string.Concat(segnalazione.MatricolaOperatore, "@inps.it");

                GestioneControlliDinamici.ControlloDinamico endpointIconaProd = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("EndpointIconaProd", out endpointIconaProd);

                //Subject
                if (endpointIconaProd != null && endpointIconaProd.ValoreControllo == "SI")
                    msgSubject = "Sviluppo - ";
                msgSubject = msgSubject + segnalazione.Procedura;
                msgSubject = msgSubject + " - Matricola: " + segnalazione.MatricolaOperatore + " - Sede: " + segnalazione.SedeOperatore.Substring(0, 6) + " - ";
                msgSubject = msgSubject + segnalazione.Tipologia + (!string.IsNullOrEmpty(segnalazione.NDomus) ? " - Domus: " + segnalazione.NDomus : string.Empty);
                msgSubject = msgSubject + (!string.IsNullOrEmpty(segnalazione.Categoria) ? " - Pensione: " + segnalazione.Categoria : string.Empty);
                msgSubject = msgSubject + (!string.IsNullOrEmpty(segnalazione.Sede) ? " - " + segnalazione.Sede : string.Empty);
                msgSubject = msgSubject + (!string.IsNullOrEmpty(segnalazione.Certificato) ? " - " + segnalazione.Certificato : string.Empty);

                invioEmailExtRequest.subject = msgSubject;
                invioEmailExtRequest.body = segnalazione.Messaggio;
                invioEmailExtRequest.sede = segnalazione.SedeOperatore.Substring(0, 6);
                invioEmailExtRequest.allegatiPresenti = false;
                invioEmailExtRequest.listaAllegati = new AllegatoInvio[0];
                invioEmailExtRequest.checkEsitoInvio = true;
                invioEmailExtRequest.PEC = false;

                GestioneControlliDinamici.ControlloDinamico abilitaIconaTest = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaIconaTest", out abilitaIconaTest);
                if (abilitaIconaTest != null && abilitaIconaTest.ValoreControllo == "SI")
                {
                    GestioneControlliDinamici.ControlloDinamico elencoMailIcona = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("ElencoMailIcona", out elencoMailIcona);
                    if (elencoMailIcona != null && !string.IsNullOrEmpty(elencoMailIcona.ValoreControllo))
                    {
                        //Destinatari
                        string[] elenco = elencoMailIcona.ValoreControllo.Split(';');
                        invioEmailExtRequest.listaDestinatari = new DestinatarioInvio[elenco.Count()];
                        DestinatarioInvio[] destinatario = new DestinatarioInvio[elenco.Count()];
                        for (int i = 0; i < elenco.Count(); i++)
                        {
                            if (i % 2 == 0)
                            {
                                destinatario[i] = new DestinatarioInvio();
                                destinatario[i].tipo = EnTipoDestinatario.TO;
                                destinatario[i].indirizzoDestinatario = elenco[i].Trim();
                                invioEmailExtRequest.listaDestinatari[i] = destinatario[i];
                            }
                            else
                            {
                                destinatario[i] = new DestinatarioInvio();
                                destinatario[i].tipo = EnTipoDestinatario.BCC;
                                destinatario[i].indirizzoDestinatario = elenco[i].Trim();
                                invioEmailExtRequest.listaDestinatari[i] = destinatario[i];
                            }
                        }
                    }
                }
                else
                {
                    //Destinatari
                    invioEmailExtRequest.listaDestinatari = new DestinatarioInvio[segnalazione.Destinatari.Count];
                    DestinatarioInvio[] destinatario = new DestinatarioInvio[segnalazione.Destinatari.Count];
                    for (int i = 0; i < segnalazione.Destinatari.Count; i++)
                    {
                        if (segnalazione.Destinatari[i] == invioEmailExtRequest.mittente)
                        {
                            destinatario[i] = new DestinatarioInvio();
                            destinatario[i].tipo = EnTipoDestinatario.BCC;
                            destinatario[i].indirizzoDestinatario = segnalazione.Destinatari[i].Trim();
                            invioEmailExtRequest.listaDestinatari[i] = destinatario[i];
                        }
                        else
                        {
                            destinatario[i] = new DestinatarioInvio();
                            destinatario[i].tipo = EnTipoDestinatario.TO;
                            destinatario[i].indirizzoDestinatario = segnalazione.Destinatari[i].Trim();
                            invioEmailExtRequest.listaDestinatari[i] = destinatario[i];
                        }
                    }
                }
                requestIcona2 = Utility.GetXmlFromObject(invioEmailExtRequest);
                InvioEmailExtResponse invioEmailExtResponse = wsIcona20Client.InvioEmailExt(invioEmailExtRequest);
                if (invioEmailExtResponse.CD_RC.ToString() != "EI")
                {
                    if (invioEmailExtResponse.errorDescription != null)
                        errori = invioEmailExtResponse.errorDescription.ToString();
                    return false;
                }
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                errori = Utility.GetMessageFromException(exception);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return false;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
            {
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.WriteError(errori);
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio Icona, method InvioEmail: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.WriteError(errori);
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                {
                    string messaggio = errori;
                    errori = "Errore tecnico durante l'invio della segnalazione";
                    string parametri = null;
                    try
                    {
                        parametri = requestIcona2;
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                }
            }
            return true;
        }

        private static bool InvioNotificaSbloccoDomandaUnicarpeIcona2(string matricolaOperatore, long nDomus, string codiceFiscale, short sede, string messaggio, string mailTo, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            string stackTrace = null;
            string msgSubject = string.Empty;
            string requestIcona2 = string.Empty;

            try
            {
                WSIcona20Client wsIcona20Client = new WSIcona20Client();
                InvioEmailExtRequest invioEmailExtRequest = new InvioEmailExtRequest();

                // chiave univoca
                Guid guid = Guid.NewGuid();
                invioEmailExtRequest.chiaveGestionale = "LIQPENS-" + guid.ToString();

                //mittente
                invioEmailExtRequest.mittente = string.Concat(matricolaOperatore, "@inps.it");

                //Subject
                msgSubject = msgSubject + "Sblocco Lavorazione Manuale";
                msgSubject = msgSubject + " - Domus: " + nDomus.ToString() + " - Sede: " + sede + " - ";
                msgSubject = msgSubject + " - Codice fiscale: " + codiceFiscale;

                invioEmailExtRequest.subject = msgSubject;
                invioEmailExtRequest.body = messaggio;
                invioEmailExtRequest.sede = sede.ToString();
                invioEmailExtRequest.allegatiPresenti = false;
                invioEmailExtRequest.listaAllegati = new AllegatoInvio[0];
                invioEmailExtRequest.checkEsitoInvio = true;
                invioEmailExtRequest.PEC = false;

                GestioneControlliDinamici.ControlloDinamico abilitaUnicarpeIconaFromDbMemo06_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaUnicarpeIconaFromDbMemo06_2024", out abilitaUnicarpeIconaFromDbMemo06_2024);
                GestioneControlliDinamici.ControlloDinamico mailUnicarpeIconaMemo06_2024 = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("MailUnicarpeIconaMemo06_2024", out mailUnicarpeIconaMemo06_2024);
                if (abilitaUnicarpeIconaFromDbMemo06_2024 != null && abilitaUnicarpeIconaFromDbMemo06_2024.ValoreControllo == "SI" &&
                    mailUnicarpeIconaMemo06_2024 != null && !string.IsNullOrEmpty(mailUnicarpeIconaMemo06_2024.ValoreControllo))
                {
                    //Destinatari
                    string mailToDb = mailUnicarpeIconaMemo06_2024.ValoreControllo;
                    invioEmailExtRequest.listaDestinatari = new DestinatarioInvio[1];
                    DestinatarioInvio[] destinatario = new DestinatarioInvio[1];
                    destinatario[0] = new DestinatarioInvio();
                    destinatario[0].tipo = EnTipoDestinatario.TO;
                    destinatario[0].indirizzoDestinatario = mailToDb;
                    invioEmailExtRequest.listaDestinatari[0] = destinatario[0];
                }
                else
                {
                    //Destinatari
                    invioEmailExtRequest.listaDestinatari = new DestinatarioInvio[1];
                    DestinatarioInvio[] destinatario = new DestinatarioInvio[1];
                    destinatario[0] = new DestinatarioInvio();
                    destinatario[0].tipo = EnTipoDestinatario.TO;
                    destinatario[0].indirizzoDestinatario = mailTo;
                    invioEmailExtRequest.listaDestinatari[0] = destinatario[0];
                }
                requestIcona2 = Utility.GetXmlFromObject(invioEmailExtRequest);
                InvioEmailExtResponse invioEmailExtResponse = wsIcona20Client.InvioEmailExt(invioEmailExtRequest);
                if (invioEmailExtResponse.CD_RC.ToString() != "EI")
                {
                    if (invioEmailExtResponse.errorDescription != null)
                    {
                        errori = invioEmailExtResponse.errorDescription.ToString();
                        erroreTecnico = true;
                        return false;
                    }
                }
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
            {
                errori = Utility.GetMessageFromException(exception);
                stackTrace = exception.StackTrace;
                erroreTecnico = true;
                return false;
            }
            catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
            {
                errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.WriteError(errori);
                return false;
            }
            catch (System.ServiceModel.EndpointNotFoundException Ex)
            {
                errori = string.Format("Puntamento errato al servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            catch (System.ServiceModel.CommunicationException Ex)
            {
                errori = string.Format("Errore di comunicazione con il servizio Icona, method InvioEmail | {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            catch (Exception Ex)
            {
                errori = string.Format("Errore nella chiamata al servizio Icona, method InvioEmail: {0}", Utility.GetMessageFromException(Ex));
                stackTrace = Ex.StackTrace;
                erroreTecnico = true;
                INPS.DNA.Logging.Logger.WriteError(errori);
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                {
                    string messaggioErrore = errori;
                    errori = "Errore tecnico durante l'invio della mail di sblocco lavorazione manuale";
                    string parametri = null;
                    try
                    {
                        parametri = requestIcona2;
                    }
                    catch (Exception)
                    {
                        // Eccezione ignorata
                    }
                    GestioneLogGenerico.SalvaLogGenerico(nDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggioErrore, parametri, stackTrace);
                }
            }
            return true;
        }
    }
}
