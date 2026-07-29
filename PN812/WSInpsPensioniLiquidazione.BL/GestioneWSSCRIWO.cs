using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.SCRIWO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneWSSCRIWO
    {
        #region public
        public static void AggiornaStatoLavorazione(GestionePensione.DatiPensione datiPensione, byte? statoDB, string matricolaOperatore, short sedeOperatore, bool isRiassegnazione = false)
        {
            try
            {
                //chiama il servizio se la domanda è automatizzata, la matricola è di un utente normale (non abilitata a simulare automazione) 
                //in fase di cambio stato domanda o presa in carico della domanda da parte di nuovo utente
               
                if (((datiPensione.TipoAutomazione != null && !GestioneCtrlMatricoleAutomazione.IsMatricolaForAutomazione(matricolaOperatore)) ||
                    (datiPensione.IsTentataAutomazione.GetValueOrDefault() && Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.GetValueOrDefault()) == Utility.StatoPensione.Calcolata)) &&
                    (statoDB != datiPensione.StatoPensione || isRiassegnazione))
                {
                    GestioneControlliDinamici.ControlloDinamico ctrlAbilitaScritturaDashboard = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaScritturaDashboard", out ctrlAbilitaScritturaDashboard);

                    if (ctrlAbilitaScritturaDashboard != null && ctrlAbilitaScritturaDashboard.ValoreControllo == "SI")
                        InsertOrUpdateFlusso(datiPensione, matricolaOperatore, isRiassegnazione);
                }

                if (ConfigurationManager.AppSettings["AggiornaSCRIWO"] == null || ConfigurationManager.AppSettings["AggiornaSCRIWO"] != "SI" || statoDB == datiPensione.StatoPensione)
                    return;
                GestioneControlliDinamici.ControlloDinamico ctrl = null;
                GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitazioneMsScriwo", out ctrl);

                if (ctrl != null && ctrl.ValoreControllo == "SI")
                    GestioneMsScriwo.AggiornaStatoLavorazione(datiPensione, statoDB, matricolaOperatore, sedeOperatore);
                else
                    AggiornaStatoLavorazionePrivate(datiPensione, statoDB, matricolaOperatore, sedeOperatore);
                return;
            }
            catch (Exception ex)
            {
                INPS.DNA.Logging.Logger.LogException(ex);
            }

        }
        #endregion

        #region private

        private static bool AggiornaStatoLavorazionePrivate(GestionePensione.DatiPensione datiPensione, byte? statoDB, string matricolaOperatore, short sedeOperatore)
        {
            WSSCRIWO_AggiornaClient proxy = null;
            StatoLavorazioneRequest request = new StatoLavorazioneRequest();

            try
            {
                GestioneLavorazione.DatiLavorazione datiLavorazione;
                GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);
                string progFase = string.Empty;
                //if (datiLavorazione != null) progFase = datiLavorazione.CodFase;

                //valorizzazione request 
                request.SistemaChiamante = ConfigurationManager.AppSettings["SistemaChiamanteSCRIWO"];
                request.TipoLavorazione = EnumsTipoLavorazione.Domanda;
                request.ChiaveLavorazione = datiPensione.NDomus.ToString();
                int sizeLav = 1;
                Lavorazione primoElementoLista = null;
                Lavorazione secondoElementoLista = null;
                Lavorazione terzoElementoLista = null;

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
                                primoElementoLista = new Lavorazione()
                                {
                                    InfoScriwo = new InfoScriwo()
                                    {
                                        StepScriwo = EnumsStepScriwo.Liquidazione,
                                        StatoScriwo = EnumsStatoScriwo.Completato
                                    },
                                    InfoSistemaChiamante = new InfoSistemaChiamante()
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
                                primoElementoLista = new Lavorazione()
                                {
                                    InfoScriwo = new InfoScriwo()
                                    {
                                        StepScriwo = EnumsStepScriwo.CalcoloPensione,
                                        StatoScriwo = EnumsStatoScriwo.NonAvviato
                                    },
                                    InfoSistemaChiamante = new InfoSistemaChiamante()
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
                                primoElementoLista = new Lavorazione()
                                {
                                    InfoScriwo = new InfoScriwo()
                                    {
                                        StepScriwo = EnumsStepScriwo.CalcoloPensione,
                                        StatoScriwo = EnumsStatoScriwo.Disattivo
                                    },
                                    InfoSistemaChiamante = new InfoSistemaChiamante()
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
                        secondoElementoLista = new Lavorazione()
                        {
                            InfoScriwo = new InfoScriwo()
                            {
                                StepScriwo = EnumsStepScriwo.CalcoloPensione,
                                StatoScriwo = EnumsStatoScriwo.InElaborazione
                            },
                            InfoSistemaChiamante = new InfoSistemaChiamante()
                            {
                                CodiceStato = datiPensione.StatoPensione.ToString(),
                                DescrizioneStato = ConvertToPascal(Utility.GetDescription(stato)),
                                DataLavorazione = DateTime.Now.AddSeconds(-1),
                                Matricola = matricolaOperatore,
                                ProgFase = progFase
                            }
                        };
                        break;
                    case Utility.StatoPensione.Calcolata:
                        secondoElementoLista = new Lavorazione()
                        {
                            InfoScriwo = new InfoScriwo()
                            {
                                StepScriwo = EnumsStepScriwo.CalcoloPensione,
                                StatoScriwo = EnumsStatoScriwo.Completato
                            },
                            InfoSistemaChiamante = new InfoSistemaChiamante()
                            {
                                CodiceStato = datiPensione.StatoPensione.ToString(),
                                DescrizioneStato = ConvertToPascal(Utility.GetDescription(stato)),
                                DataLavorazione = DateTime.Now.AddSeconds(-1),
                                Matricola = matricolaOperatore,
                                ProgFase = progFase
                            }
                        };
                        break;
                    case Utility.StatoPensione.InAcquisizione:
                        secondoElementoLista = new Lavorazione()
                        {
                            InfoScriwo = new InfoScriwo()
                            {
                                StepScriwo = EnumsStepScriwo.Liquidazione,
                                StatoScriwo = EnumsStatoScriwo.InElaborazione
                            },
                            InfoSistemaChiamante = new InfoSistemaChiamante()
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
                        secondoElementoLista = new Lavorazione()
                        {
                            InfoScriwo = new InfoScriwo()
                            {
                                StepScriwo = EnumsStepScriwo.Liquidazione,
                                StatoScriwo = EnumsStatoScriwo.NonAvviato
                            },
                            InfoSistemaChiamante = new InfoSistemaChiamante()
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
                                                                                            ,Utility.StatoPensione.CalcolataNoNoteDebito};

                if (stato != null && secondoElementoLista != null && StatiFinali.Contains((Utility.StatoPensione)stato))
                {
                    var codAttivita = GestioneWebDom.GetAttivitaDiChiusura(datiPensione);
                    if (codAttivita == GestioneWebDom.CodiceAttivita.CalcoloProvvisorio || codAttivita == GestioneWebDom.CodiceAttivita.CalcoloProvvisorioEMENS || codAttivita == GestioneWebDom.CodiceAttivita.CalcoloProvvisorioDMAG)
                    {
                        secondoElementoLista.InfoSistemaChiamante.IsProvvisoria = true;

                        //Se lo stato precedente non è uno stato "finale" e il nuovo stato è "finale", posso inviare l'evento di trasformazione
                        if (statoDB != null && !StatiFinali.Contains((Utility.StatoPensione)statoDB))
                        {
                            terzoElementoLista = new Lavorazione()
                            {
                                InfoScriwo = new InfoScriwo()
                                {
                                    StepScriwo = EnumsStepScriwo.VerificaDomanda,
                                    StatoScriwo = EnumsStatoScriwo.InElaborazione
                                },
                                InfoSistemaChiamante = new InfoSistemaChiamante()
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

                request.Lavorazioni = new Lavorazione[sizeLav];

                if (primoElementoLista != null)
                {
                    request.Lavorazioni.SetValue(primoElementoLista, sizeLav - 2 - adeguaCount);
                }

                request.Lavorazioni.SetValue(secondoElementoLista, sizeLav - 1 - adeguaCount);

                if (terzoElementoLista != null)
                {
                    request.Lavorazioni.SetValue(terzoElementoLista, sizeLav - 1);
                }

                proxy = new WSSCRIWO_AggiornaClient();
                //var res = proxy.AggiornaStatoLavorazione(request);
                proxy.AggiornaStatoLavorazioneOneWay(request);
            }

            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);

                return false;
            }
            finally
            {
                Utility.CloseClient(proxy);
            }

            return true;
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

        private static void InsertOrUpdateFlusso(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, bool isRiassegnazione)
        {

            try
            {

                GestioneAnagrafica.DatiAnagrafici anagrafica;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out anagrafica);

                if (datiPensione != null)
                {
                    GestioneMsScriwo.InsertOrUpdateFlusso(datiPensione, anagrafica, matricolaOperatore, isRiassegnazione);
                }

            }
            catch (Exception Ex)
            {
                INPS.DNA.Logging.Logger.LogException(Ex);
                string messaggio = Utility.GetMessageFromException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);
            }
        }

        #endregion

    }
}
