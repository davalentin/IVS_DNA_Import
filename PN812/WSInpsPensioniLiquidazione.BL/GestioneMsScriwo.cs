using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Net;
using System.Text;
using INPS.Pensioni.Liquidazione.Service_Reference;
using System.Reflection;
using System.Linq;


namespace INPS.Pensioni.Liquidazione
{
    public class GestioneMsScriwo
    {
        #region public
        public static void AggiornaStatoLavorazione(GestionePensione.DatiPensione datiPensione, byte? statoDB, string matricolaOperatore, short sedeOperatore)
        {
            try
            {
                if (ConfigurationManager.AppSettings["AggiornaSCRIWO"] == null || ConfigurationManager.AppSettings["AggiornaSCRIWO"] != "SI" || statoDB == datiPensione.StatoPensione)
                    return;

                AggiornaStatoLavorazionePrivate(datiPensione, statoDB, matricolaOperatore, sedeOperatore);
                return;
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
            }

        }

        public static void InsertOrUpdateFlusso(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici anagrafica, string matricolaOperatore, bool isRiassegnazione)
        {
            try
            {
                InsertOrUpdateFlussoPrivate(datiPensione, anagrafica, matricolaOperatore, isRiassegnazione);
                return;
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
            }
        }
        #endregion

        #region private


        private static WebClient SetApiIdentity(string matricola)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient client = new WebClient();
            string token = string.Empty;
            TokenIdentity identity = new TokenIdentity();
            identity.UserId = matricola ?? identity.UserId;
            var plainIdentity = JsonConvert.SerializeObject(identity);
            var encoding = Encoding.UTF8;
            var bytesIdentity = encoding.GetBytes(plainIdentity);
            token = string.Concat(TokenBearer, TokenHeader, ".", Convert.ToBase64String(bytesIdentity), ".");
            client.Headers.Add(ApiClientId, ConfigurationManager.AppSettings[Config_ApiClientId] ?? string.Empty);
            client.Headers.Add(ApiClientSecret, ConfigurationManager.AppSettings[Config_ApiClientSecret] ?? string.Empty);
            client.Headers.Add(ApiAuthorization, token);
            client.Headers.Add("Content-Type", "application/json");

            return client;
        }
        private static bool AggiornaStatoLavorazionePrivate(GestionePensione.DatiPensione datiPensione, byte? statoDB, string matricolaOperatore, short sedeOperatore)
        {
            msScriwoClient.StatoLavorazioneRequest request = new msScriwoClient.StatoLavorazioneRequest();

            try
            {
                GestioneLavorazione.DatiLavorazione datiLavorazione;
                GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
                string progFase = string.Empty;
                //if (datiLavorazione != null) progFase = datiLavorazione.CodFase;

                //valorizzazione request 
                request.SistemaChiamante = ConfigurationManager.AppSettings["SistemaChiamanteSCRIWO"];
                request.TipoLavorazione = msScriwoClient.TipoLavorazione.Domanda;
                request.ChiaveLavorazione = datiPensione.NDomus.ToString();
                int sizeLav = 1;
                msScriwoClient.Lavorazione primoElementoLista = null;
                msScriwoClient.Lavorazione secondoElementoLista = null;
                msScriwoClient.Lavorazione terzoElementoLista = null;

                Utility.StatoPensione? stato = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());

                if (statoDB != null)
                {
                    Utility.StatoPensione? statoPrec = Utility.GetStatoPensioneByCodice((byte)statoDB);
                    switch (statoPrec.GetValueOrDefault())
                    {
                        case Utility.StatoPensione.DaAcquisire:
                        case Utility.StatoPensione.InAcquisizione:
                            if (stato != Utility.StatoPensione.DaAcquisire) //solo se non è il caso di cancellazione domanda 
                            {
                                primoElementoLista = new msScriwoClient.Lavorazione()
                                {
                                    InfoScriwo = new msScriwoClient.InfoScriwo()
                                    {
                                        StepScriwo = msScriwoClient.StepScriwo.Liquidazione,
                                        StatoScriwo = msScriwoClient.StatoScriwo.Completato
                                    },
                                    InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                                    {
                                        CodiceStato = statoDB.ToString(),
                                        DescrizioneStato = ConvertToPascal(Utility.GetDescription(statoPrec)),
                                        DataLavorazione = DateTime.Now.AddSeconds(-1),
                                        Matricola = matricolaOperatore,
                                        ProgFase = progFase
                                    }
                                };
                                sizeLav++;
                            }
                            break;
                        case Utility.StatoPensione.CalcolataNoWebDom:
                        case Utility.StatoPensione.CalcolataNoFelpe:
                        case Utility.StatoPensione.CalcolataNoOneri:
                        case Utility.StatoPensione.CalcolataNoSAI:
                        case Utility.StatoPensione.CalcolataNoStazLavoro:
                        case Utility.StatoPensione.CalcolataNoTotal:
                        case Utility.StatoPensione.CalcolataNoTot:
                        case Utility.StatoPensione.CalcolataNoSIN:
                        case Utility.StatoPensione.DaCalcolare:
                        case Utility.StatoPensione.CalcoloVerify:
                        case Utility.StatoPensione.ScartoVerify:
                        case Utility.StatoPensione.ScartoDaCalcolo:
                        case Utility.StatoPensione.CalcolataNoBooking:
                        case Utility.StatoPensione.CalcolataNoNoteDebito:
                        case Utility.StatoPensione.CalcolataNo6Scatti:
                            if (stato == Utility.StatoPensione.InAcquisizione)
                            {
                                primoElementoLista = new msScriwoClient.Lavorazione()
                                {
                                    InfoScriwo = new msScriwoClient.InfoScriwo()
                                    {
                                        StepScriwo = msScriwoClient.StepScriwo.CalcoloPensione,
                                        StatoScriwo = msScriwoClient.StatoScriwo.NonAvviato
                                    },
                                    InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                                    {
                                        CodiceStato = statoDB.ToString(),
                                        DescrizioneStato = ConvertToPascal(Utility.GetDescription(statoPrec)),
                                        DataLavorazione = DateTime.Now.AddSeconds(-1),
                                        Matricola = matricolaOperatore,
                                        ProgFase = progFase
                                    }
                                };
                                sizeLav++;
                            }
                            if (stato == Utility.StatoPensione.DaAcquisire)
                            {
                                primoElementoLista = new msScriwoClient.Lavorazione()
                                {
                                    InfoScriwo = new msScriwoClient.InfoScriwo()
                                    {
                                        StepScriwo = msScriwoClient.StepScriwo.CalcoloPensione,
                                        StatoScriwo = msScriwoClient.StatoScriwo.Disattivo
                                    },
                                    InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                                    {
                                        CodiceStato = statoDB.ToString(),
                                        DescrizioneStato = "Annullato",
                                        DataLavorazione = DateTime.Now.AddSeconds(-1),
                                        Matricola = matricolaOperatore,
                                        ProgFase = progFase
                                    }
                                };
                                sizeLav++;
                            }
                            break;

                    }
                }


                switch (stato.GetValueOrDefault())
                {

                    case Utility.StatoPensione.CalcolataNoWebDom:
                    case Utility.StatoPensione.CalcolataNoFelpe:
                    case Utility.StatoPensione.CalcolataNoOneri:
                    case Utility.StatoPensione.CalcolataNoSAI:
                    case Utility.StatoPensione.CalcolataNoStazLavoro:
                    case Utility.StatoPensione.CalcolataNoTotal:
                    case Utility.StatoPensione.CalcolataNoTot:
                    case Utility.StatoPensione.CalcolataNoSIN:
                    case Utility.StatoPensione.DaCalcolare:
                    case Utility.StatoPensione.CalcoloVerify:
                    case Utility.StatoPensione.ScartoVerify:
                    case Utility.StatoPensione.CalcolataNoBooking:
                    case Utility.StatoPensione.CalcolataNoNoteDebito:
                    case Utility.StatoPensione.ScartoDaCalcolo:
                    case Utility.StatoPensione.CalcolataNo6Scatti:
                        secondoElementoLista = new msScriwoClient.Lavorazione()
                        {
                            InfoScriwo = new msScriwoClient.InfoScriwo()
                            {
                                StepScriwo = msScriwoClient.StepScriwo.CalcoloPensione,
                                StatoScriwo = msScriwoClient.StatoScriwo.InElaborazione
                            },
                            InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                            {
                                CodiceStato = datiPensione.StatoPensione.ToString(),
                                DescrizioneStato = ConvertToPascal(Utility.GetDescription(stato)),
                                DataLavorazione = DateTime.Now,
                                Matricola = matricolaOperatore,
                                ProgFase = progFase
                            }
                        };
                        break;
                    case Utility.StatoPensione.Calcolata:
                        secondoElementoLista = new msScriwoClient.Lavorazione()
                        {
                            InfoScriwo = new msScriwoClient.InfoScriwo()
                            {
                                StepScriwo = msScriwoClient.StepScriwo.CalcoloPensione,
                                StatoScriwo = msScriwoClient.StatoScriwo.Completato
                            },
                            InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                            {
                                CodiceStato = datiPensione.StatoPensione.ToString(),
                                DescrizioneStato = ConvertToPascal(Utility.GetDescription(stato)),
                                DataLavorazione = DateTime.Now,
                                Matricola = matricolaOperatore,
                                ProgFase = progFase
                            }
                        };
                        break;
                    case Utility.StatoPensione.InAcquisizione:
                        secondoElementoLista = new msScriwoClient.Lavorazione()
                        {
                            InfoScriwo = new msScriwoClient.InfoScriwo()
                            {
                                StepScriwo = msScriwoClient.StepScriwo.Liquidazione,
                                StatoScriwo = msScriwoClient.StatoScriwo.InElaborazione
                            },
                            InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                            {
                                CodiceStato = datiPensione.StatoPensione.ToString(),
                                DescrizioneStato = ConvertToPascal(Utility.GetDescription(stato)),
                                DataLavorazione = DateTime.Now,
                                Matricola = matricolaOperatore,
                                ProgFase = progFase
                            }
                        };
                        break;
                    case Utility.StatoPensione.DaAcquisire:
                        secondoElementoLista = new msScriwoClient.Lavorazione()
                        {
                            InfoScriwo = new msScriwoClient.InfoScriwo()
                            {
                                StepScriwo = msScriwoClient.StepScriwo.Liquidazione,
                                StatoScriwo = msScriwoClient.StatoScriwo.NonAvviato
                            },
                            InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                            {
                                CodiceStato = datiPensione.StatoPensione.ToString(),
                                DescrizioneStato = ConvertToPascal(Utility.GetDescription(stato)),
                                DataLavorazione = DateTime.Now,
                                Matricola = matricolaOperatore,
                                ProgFase = progFase
                            }
                        };
                        break;
                }

                //Verifica domanda Provvisoria
                int adeguaCount = 0;
                List<Utility.StatoPensione> StatiFinali = new List<Utility.StatoPensione>() { Utility.StatoPensione.Calcolata
                                                                                            ,Utility.StatoPensione.CalcolataNoWebDom
                                                                                            ,Utility.StatoPensione.CalcolataNoFelpe
                                                                                            ,Utility.StatoPensione.CalcolataNoOneri
                                                                                            ,Utility.StatoPensione.CalcolataNoSAI
                                                                                            ,Utility.StatoPensione.CalcolataNoStazLavoro
                                                                                            ,Utility.StatoPensione.CalcolataNoTotal
                                                                                            ,Utility.StatoPensione.CalcolataNoTot
                                                                                            ,Utility.StatoPensione.CalcolataNoSIN
                                                                                            ,Utility.StatoPensione.CalcolataNoBooking
                                                                                            ,Utility.StatoPensione.CalcolataNoNoteDebito
                                                                                            ,Utility.StatoPensione.CalcolataNo6Scatti};

                if (stato != null && secondoElementoLista != null && StatiFinali.Contains((Utility.StatoPensione)stato))
                {
                    var codAttivita = GestioneWebDom.GetAttivitaDiChiusura(datiPensione);
                    if (codAttivita == GestioneWebDom.CodiceAttivita.CalcoloProvvisorio || codAttivita == GestioneWebDom.CodiceAttivita.CalcoloProvvisorioEMENS || codAttivita == GestioneWebDom.CodiceAttivita.CalcoloProvvisorioDMAG)
                    {
                        secondoElementoLista.InfoSistemaChiamante.IsProvvisoria = true;

                        //Se lo stato precedente non è uno stato "finale" e il nuovo stato è "finale", posso inviare l'evento di trasformazione
                        if (statoDB != null && !StatiFinali.Contains((Utility.StatoPensione)statoDB))
                        {
                            terzoElementoLista = new msScriwoClient.Lavorazione()
                            {
                                InfoScriwo = new msScriwoClient.InfoScriwo()
                                {
                                    StepScriwo = msScriwoClient.StepScriwo.VerificaDomanda,
                                    StatoScriwo = msScriwoClient.StatoScriwo.InElaborazione
                                },
                                InfoSistemaChiamante = new msScriwoClient.InfoSistemaChiamante()
                                {
                                    CodiceStato = "Trasformazione",
                                    DescrizioneStato = "Avvio Trasformazione a seguito di Liquidazione provvisoria",
                                    DataLavorazione = secondoElementoLista.InfoSistemaChiamante.DataLavorazione.AddSeconds(1),
                                    Matricola = matricolaOperatore,
                                    ProgFase = progFase
                                }
                            };
                            sizeLav++;
                            adeguaCount++;
                        }
                    }
                }

                msScriwoClient.Lavorazione[] temp = new msScriwoClient.Lavorazione[sizeLav];

                if (primoElementoLista != null)
                {
                    temp.SetValue(primoElementoLista, sizeLav - 2 - adeguaCount);
                }

                temp.SetValue(secondoElementoLista, sizeLav - 1 - adeguaCount);

                if (terzoElementoLista != null)
                {
                    temp.SetValue(terzoElementoLista, sizeLav - 1);
                }


                request.Lavorazioni = temp.ToList();

                msScriwoClient apiScriwo = new msScriwoClient(SetApiIdentity(matricolaOperatore));
                apiScriwo.AggiornaStatoLavorazione(request);


            }

            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);

                return false;
            }

            return true;
        }

        private static bool InsertOrUpdateFlussoPrivate(GestionePensione.DatiPensione datiPensione, GestioneAnagrafica.DatiAnagrafici anagrafica, string matricolaOperatore, bool isRiassegnazione)
        {
            Utility.StatoPensione? stato = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault());
            msSistemaPensioni.AreaDomandeAutomatizzateRequest request = new msSistemaPensioni.AreaDomandeAutomatizzateRequest();
            try
            {
                msSistemaPensioni.DomandaAutomatizzataEnitity domanda = new msSistemaPensioni.DomandaAutomatizzataEnitity
                {
                    NumDomus = datiPensione.NDomus,
                    DataElaborazioneOperatore = DateTime.Now,
                    DescrizioneEsitoOperatore = GetSmStato(stato.GetValueOrDefault(), datiPensione.IsTentataAutomazione),
                    DettaglioOperatore = isRiassegnazione ? "Domanda presa in carico da nuovo utente" : Utility.GetDescription(stato),
                    MatricolaOperatore = matricolaOperatore,
                    DataNascita = anagrafica != null ? anagrafica.DataNascita : null,
                    DecorrenzaPensione = datiPensione.DecorrenzaOriginaria,
                    ChiavePensione = datiPensione.GetCodCategoria().Substring(1, 3) + "-" +
                        (datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value.ToString().PadLeft(4, '0') : datiPensione.CodiceSede.ToString().PadLeft(4, '0')) + "-" +
                        datiPensione.NCertificato.ToString().PadLeft(8, '0')
                };

                msSistemaPensioni.AreaUtente areaUtente = new msSistemaPensioni.AreaUtente()
                {
                    IdConsumer = 1
                };

                request.Domanda = domanda;
                request.Utente = areaUtente;

                msSistemaPensioni apiSistemaPensioni = new msSistemaPensioni(SetApiIdentity(matricolaOperatore));

                Guid guid = Guid.NewGuid();
                GestioneLogSoap.SalvaLogSoap(request, Utility.Servizio.SrvSistemaPensioni, Utility.MetodoServizio.InsertOrUpdateDashboard, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                apiSistemaPensioni.InsertOrUpdateFlusso(request);

            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);

                return false;
            }
            return true;
        }

        private static string GetSmStato(Utility.StatoPensione? stato, bool? isTentataAutomazione)
        {
            string result = "DA COMPLETARE";

            switch (stato.GetValueOrDefault())
            {
                case Utility.StatoPensione.Calcolata:
                    if(isTentataAutomazione.GetValueOrDefault()) result = "OK SENZA AUTOMAZIONE";
                    else result = "OK";
                    break;
                case Utility.StatoPensione.ScartoVerify:
                case Utility.StatoPensione.ScartoDaCalcolo:
                    result = "KO APPLICATIVO";
                    break;
                case Utility.StatoPensione.DaAcquisire:
                    result = "CANCELLATA";
                    break;
            }
            return result;
        }

        private static string ConvertToPascal(string inputString)
        {
            if (inputString == null)
                return string.Empty;

            string[] words = inputString.Split(' ');

            StringBuilder returnStr = new StringBuilder();

            foreach (string word in words)
            {
                if (word.Length > 1)
                {
                    returnStr.Append(word.Substring(0, 1).ToUpper());
                    returnStr.Append(word.Substring(1).ToLower());
                    returnStr.Append(" ");
                }
                else
                {
                    returnStr.Append(word);
                    returnStr.Append(" ");
                }
            }
            return returnStr.ToString().Trim();
        }
        private class TokenIdentity
        {
            public TokenIdentity()
            {
                IdentityProvider = ConfigurationManager.AppSettings[Config_ApiScriwoProvider] != null ? ConfigurationManager.AppSettings[Config_ApiScriwoProvider].ToString() : string.Empty;
                UserId = ConfigurationManager.AppSettings[Config_ApiScriwoUserId] != null ? ConfigurationManager.AppSettings[Config_ApiScriwoUserId].ToString() : string.Empty;
                CodiceEnte = string.Empty;
                CodiceUfficio = string.Empty;
            }
            public string UserId { get; set; }
            public string IdentityProvider { get; set; }
            public string CodiceEnte { get; set; }
            public string CodiceUfficio { get; set; }
        }

        private const string TokenHeader = "eyJhbGciOiJub25lIn0";
        private const string TokenBearer = "Bearer ";
        private const string ApiClientId = "X-IBM-Client-Id";
        private const string ApiClientSecret = "X-IBM-Client-Secret";
        private const string ApiAuthorization = "Authorization";
        private const string Config_ApiClientId = "ApiClientId";
        private const string Config_ApiClientSecret = "ApiClientSecret";
        private const string Config_ApiScriwoProvider = "ApiScriwoProvider";
        private const string Config_ApiScriwoUserId = "ApiScriwoUserId";
        #endregion
    }
}
