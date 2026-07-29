using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.ServiceReferences.WebDom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;


namespace INPS.Pensioni.Liquidazione
{
    public class GestioneWebDom
    {
        #region public members
        public static void AperturaAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, CodiceAttivita codAttivita, out string errori)
        {
            errori = string.Empty;

            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                return;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                int indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 1;
                string codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);

                if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CollegamentoConOrganismoEstero)
                {
                    indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 2;
                    codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                }

                if (!VerificaCompatibilitaAperturaAttivita(codAttivita, codAttivitaPrec, out errori))
                    return;

                bool IsAppenaChiusa = false;
                if (codAttivita == CodiceAttivita.InAcquisizione)
                {
                    if (!FineUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, datiDomanda, out IsAppenaChiusa, out errori))
                        return;
                }

                if (IsAppenaChiusa)
                {
                    if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                        return;
                }

                if (!InizioAttivita(datiPensione, matricolaOperatore, codAttivita, datiDomanda, out errori))
                    return;
            }
            else if (codAttivita == CodiceAttivita.InAcquisizione)
            {
                if (!InizioAttivita(datiPensione, matricolaOperatore, codAttivita, datiDomanda, out errori))
                    return;
            }
            else
            {
                errori = "Controlli per WebDom: L'attività che si sta tentando di inserire '" + codAttivita.ToString() + "' (" + Utility.GetDescription(codAttivita) +
                    ") risulta errata. E' possibile inserire soltanto l'attività '" + CodiceAttivita.InAcquisizione.ToString() + "' (" + Utility.GetDescription(CodiceAttivita.InAcquisizione) + ")";
                return;
            }
        }

        public static void AperturaPrimaAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, CodiceAttivita codAttivita, Utility.TipoAppartenenza? tipoAppartenenza, out string errori)
        {
            errori = string.Empty;

            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                return;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                string codAttivitaPrec = GetCodiceAttivita(datiDomanda, datiDomanda.Dati.Attivita.Rows.Count - 1);
                if (!VerificaCompatibilitaAperturaAttivita(codAttivita, codAttivitaPrec, out errori))
                    return;

                bool IsAppenaChiusa = false;
                if (codAttivita == CodiceAttivita.InAcquisizione)
                {
                    if (!FineUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, datiDomanda, out IsAppenaChiusa, out errori))
                        return;
                }

                if (IsAppenaChiusa)
                {
                    if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                        return;
                }

                if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI)
                {
                    //18-03-2014: per PL se non presente IterCI messaggioBloccante, per RIC nessun controllo
                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Ricostituzione && !(datiDomanda.Dati != null && datiDomanda.Dati.Fase != null && datiDomanda.Dati.Fase.Count > 0 &&
                        (datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0060" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0062" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0063")))
                    {
                        bool IsPresenteIterCI = false;
                        for (int i = datiDomanda.Dati.Attivita.Rows.Count - 1; i >= 0; i--)
                        {
                            string codAttivitaCI = GetCodiceAttivita(datiDomanda, i);
                            if (int.Parse(codAttivitaCI) == (int)CodiceAttivita.AvvioIterCI &&
                                (datiDomanda.Dati.Attivita[i]["DataFine"] == DBNull.Value || string.IsNullOrEmpty(datiDomanda.Dati.Attivita[i]["DataFine"].ToString())))
                            {
                                IsPresenteIterCI = true;
                                break;
                            }
                        }
                        if (!IsPresenteIterCI)
                        {
                            errori = "Controlli per WebDom: Domanda non lavorabile, attività WebDom " + Utility.GetDescription(CodiceAttivita.AvvioIterCI) + " non presente";
                            return;
                            //if (!InizioAttivita(datiPensione.NDomus, matricolaOperatore, sedeOperatore, CodiceAttivita.AvvioIterCI, datiDomanda, out errori))
                            //    return;

                            //if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                            //    return;
                        }
                    }
                }

                if (!InizioAttivita(datiPensione, matricolaOperatore, codAttivita, datiDomanda, out errori))
                    return;
            }
            else if (codAttivita == CodiceAttivita.InAcquisizione)
            {
                if (tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.CI)
                {
                    if (!InizioAttivita(datiPensione, matricolaOperatore, CodiceAttivita.AvvioIterCI, datiDomanda, out errori))
                        return;

                    if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                        return;
                }

                if (!InizioAttivita(datiPensione, matricolaOperatore, codAttivita, datiDomanda, out errori))
                    return;
            }
            else
            {
                errori = "Controlli per WebDom: L'attività che si sta tentando di inserire '" + codAttivita.ToString() + "' (" + Utility.GetDescription(codAttivita) +
                    ") risulta errata. E' possibile inserire soltanto l'attività '" + CodiceAttivita.InAcquisizione.ToString() + "' (" + Utility.GetDescription(CodiceAttivita.InAcquisizione) + ")";
                return;
            }
        }

        public static void ChiusuraAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, CodiceAttivita codAttivita, out string errori)
        {
            errori = string.Empty;

            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            if (!GetDomandaPerDomus(datiPensione.NDomus.ToString(), out datiDomanda, out errori))
                return;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {

                string codAttivitaPrec = GetCodiceAttivita(datiDomanda, datiDomanda.Dati.Attivita.Rows.Count - 1);

                if (codAttivitaPrec.Equals("0"))
                    return;


                if (codAttivita == CodiceAttivita.CalcoloErrato && int.Parse(codAttivitaPrec) != (int)CodiceAttivita.CalcoloErrato)
                    return;
                else
                {
                    string dataFineAttivitaPrec = string.Empty;
                    try
                    {
                        dataFineAttivitaPrec = datiDomanda.Dati.Attivita[datiDomanda.Dati.Attivita.Rows.Count - 1]["DataFine"] != DBNull.Value ?
                            datiDomanda.Dati.Attivita[datiDomanda.Dati.Attivita.Rows.Count - 1].DataFine : string.Empty;
                    }
                    catch (Exception)
                    {
                        //Eccezione ignorata
                    }

                    if (codAttivita == CodiceAttivita.InAcquisizione && int.Parse(codAttivitaPrec) == (int)CodiceAttivita.InAcquisizione &&
                            !string.IsNullOrEmpty(dataFineAttivitaPrec))
                    {
                        errori = string.Empty;
                        return;
                    }

                    if (codAttivita != CodiceAttivita.InAttesaDiCalcoloAutomatico && !VerificaCompatibilitaChiusuraAttivita(codAttivita, codAttivitaPrec, out errori))
                    {
                        //se si ha uno stato inconsistente prima del calcolo, lo correggo in modo da non bloccare il calcolo stesso
                        //chiudo ultima attività e provo ad aprire e chiudere attesaCalcolo
                        if (codAttivita == CodiceAttivita.AttesaCalcolo && int.Parse(codAttivitaPrec) == (int)CodiceAttivita.InAcquisizione)
                        {
                            if (string.IsNullOrEmpty(dataFineAttivitaPrec))
                            {
                                if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                                    return;
                                if (!FineAttivita(datiPensione, matricolaOperatore, codAttivitaPrec, datiDomanda, out errori))
                                    return;
                            }
                            AperturaAttivita(datiPensione, matricolaOperatore, sedeOperatore, CodiceAttivita.AttesaCalcolo, out errori);
                            if (!string.IsNullOrEmpty(errori))
                                return;
                            ChiusuraAttivita(datiPensione, matricolaOperatore, sedeOperatore, CodiceAttivita.AttesaCalcolo, out errori);
                            if (!string.IsNullOrEmpty(errori))
                                return;
                        }
                        return;
                    }
                    if (string.IsNullOrEmpty(dataFineAttivitaPrec))
                    {
                        if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                            return;
                        if (!FineAttivita(datiPensione, matricolaOperatore, codAttivita, datiDomanda, out errori))
                            return;
                    }
                }

                //gestione transizione tra calcolo errato - attesa calcolo sull'invio al calcolo prima dell'esito del calcolo
                if (codAttivita == CodiceAttivita.CalcoloErrato && int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloErrato)
                    AperturaAttivita(datiPensione, matricolaOperatore, sedeOperatore, CodiceAttivita.AttesaCalcolo, out errori);
            }
            else
            {
                errori = "Controlli per WebDom: Nessuna attività disponibile per la chiusura. Non è possibile chiudere l'attività '" + codAttivita.ToString() +
                    "' (" + Utility.GetDescription(codAttivita) + ")";
                return;
            }
        }

        public static void ChiusuraUltimaAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, out string errori)
        {
            errori = string.Empty;

            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                return;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                bool IsAppenaChiusa = false;
                FineUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, datiDomanda, out IsAppenaChiusa, out errori);

                if (!IsAppenaChiusa)
                {
                    if (!SbloccaDomanda(datiPensione.NDomus, matricolaOperatore, datiDomanda.TimestampBlocco, out errori))
                        return;

                    GestioneSbloccoDomanda.DatiSbloccoDomanda datiSbloccoDomanda = new GestioneSbloccoDomanda.DatiSbloccoDomanda();
                    datiSbloccoDomanda.NDomus = datiPensione.NDomus;
                    datiSbloccoDomanda.MatricolaBlocco = matricolaOperatore;
                    datiSbloccoDomanda.TimeStampBlocco = datiDomanda.TimestampBlocco;
                    GestioneSbloccoDomanda.EliminaSbloccoDomanda(datiSbloccoDomanda);
                }
            }
        }

        public static void AggiornamentoFaseAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, out string errori, ServiceReferences.WebDom.DatiDomanda datiDomanda = null)
        {
            errori = string.Empty;

            CodiceAttivita codAttivita = GetAttivitaDiChiusura(datiPensione);

            if (codAttivita != CodiceAttivita.CalcoloEsatto &&
                codAttivita != CodiceAttivita.CalcoloProvvisorio &&
                codAttivita != CodiceAttivita.CalcoloProvvisorioEMENS &&
                codAttivita != CodiceAttivita.CalcoloProvvisorioDMAG)
            {
                errori = "Controlli per WebDom: l'attività '" + codAttivita.ToString() + "' non è conforme alla richiesta di aggiornamento di Fase e Attivita";
                return;
            }


            if (datiDomanda == null && !GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
            {
                //nel caso di errore nel lock domanda ma ultima attività pari a 0200, 0201, 0212, 0231 occorre eseguire il bypass dell' aggiornamento fase attività
                VerificaBypassAggiornamentoFaseAttivita(datiDomanda, ref errori);
                return;
            }
            bool bypassAggiornamento = false;
            if (!VerificaCompatibilitaAggiornamentoFaseAttivita(datiPensione, matricolaOperatore, sedeOperatore, ref datiDomanda, out bypassAggiornamento, out errori))
                return;
            if (bypassAggiornamento)
                return;
            if (!AggiornaFaseAttivita(datiPensione, matricolaOperatore, codAttivita, datiDomanda, out errori))
                return;
        }

        public static bool AggiornaWebDom(GestionePensione.DatiPensione datiPensione, BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa, string matricolaOperatore, short sedeOperatore, out string statoPensione,
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            statoPensione = string.Empty;
            bool isCodiceEsito9 = false;

            GestioneWebDom.SbloccaDomandaWebDom(datiPensione.NDomus, out messaggioVideo);
            messaggioVideo = string.Empty;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (!ControllaStatoPensionePerAggiornamento(datiPensione))
            {
                messaggioVideo = "Stato Pensione non valido per eseguire l'aggiornamento WebDom";
                return false;
            }

            AggiornamentoFaseAttivita(datiPensione, datiPensione.MatricolaUtenteAcquisizione, sedeOperatore, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
            {
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom);
                return false;
            }

            //ENG - ENPALS: TRF MANUALI PRECOCI
            if (!(tipoAppartenenza.HasValue && tipoAppartenenza.Value == Utility.TipoAppartenenza.AGO && Utility.IsRiaperturaDomanda(datiPensione.Id) && Utility.IsDomandaUnicarpe(datiPensione, true) == Utility.TipoUnicarpe.Manuale && Utility.IsDomandaENPALS(datiPensione.Gestione) && Utility.IsDomandaAPEPrecoci(datiPensione)))
            {
                if (!GestioneCalcoloDomanda.AggiornaFelpeDopoWebDom(datiPensione, matricolaOperatore, sedeOperatore, ref messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoFelpe;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoFelpe);
                    messaggioVideo = "Aggiornamento WebDom riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Felpe. " + messaggioVideo;
                    return false;
                }
            }

            if (!GestioneOneriPrepensionamento.AggiornaOneri(datiPensione, out messaggioVideo))
            {
                datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoOneri;
                GestionePensione.SalvaPensione(datiPensione);
                statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoOneri);
                messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Oneri. " + messaggioVideo;
                return false;
            }

            if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiPensione.IsDatiENPALSRecuperati.GetValueOrDefault())
            {
                if (!GestioneSAI.AggiornaSAI(datiPensione, datiDanteCausa, GestioneSAI.GetTipoRichiestaPAG(datiPensione), out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSAI;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSAI);
                    messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SAI. " + messaggioVideo;
                    return false;
                }
            }

            if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
            {
                if (!GestioneINPDAP.AggiornaINPDAP(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoSIN;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoSIN);
                    messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento SIN. " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Note di debito. " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaPianiDiPagamento(datiPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNo6Scatti;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNo6Scatti);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaEquoIndennizzo(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoEquoInd;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoEquoInd);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }
                if (!GestioneINPDAP.AggiornaIndennitaSpeciale(datiPensione, out messaggioVideo, out isCodiceEsito9))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoIndennSpec;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoIndennSpec);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Piani di pagamento " + messaggioVideo;
                    return false;
                }
            }


            if (Utility.IsDomandaCumulo(datiPensione.SiglaCategoria) && datiPensione.IsCumuloAutomatica.GetValueOrDefault())
            {
                if (!GestioneTotalIvs.AggiornaCumulo(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTotal;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTotal);
                    messaggioVideo = "Aggiornamento WebDom e Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento TOTAL (Cumulo). " + messaggioVideo;
                    return false;
                }
            }

            if (Utility.IsDomandaTotalizzazione(datiPensione.SiglaCategoria) && datiPensione.IsTotAutomatica.GetValueOrDefault())
            {
                if (!GestioneTotalIvs.AggiornaTot(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoTot;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoTot);
                    messaggioVideo = "Aggiornamento WebDom riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento TOTAL (Totalizzazione). " + messaggioVideo;
                    return false;
                }
            }

            if (Utility.IsDomandaMiglioramentiContrattuali(datiPensione))
            {
                if (!GestioneINPDAP.AggiornaNoteDiDebito(datiPensione, out messaggioVideo))
                {
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.CalcolataNoNoteDebito;
                    GestionePensione.SalvaPensione(datiPensione);
                    statoPensione = Utility.GetDescription(Utility.StatoPensione.CalcolataNoNoteDebito);
                    messaggioVideo = "Aggiornamento Felpe riuscito correttamente. Tuttavia si sono riscontrati problemi nel successivo aggiornamento Note di debito. " + messaggioVideo;
                    return false;
                }
            }

            datiPensione.StatoPensione = (int)Utility.StatoPensione.Calcolata;
            GestionePensione.SalvaPensione(datiPensione);

            statoPensione = Utility.GetDescription(Utility.StatoPensione.Calcolata);

            return true;
        }

        public static bool SbloccaDomandaWebDom(long numeroDomanda, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneSbloccoDomanda.DatiSbloccoDomanda datiSbloccoDomanda = null;
            GestioneSbloccoDomanda.GetSbloccoDomandaByNumeroDomanda(numeroDomanda, out datiSbloccoDomanda);
            if (datiSbloccoDomanda == null || string.IsNullOrEmpty(datiSbloccoDomanda.MatricolaBlocco) ||
                string.IsNullOrEmpty(datiSbloccoDomanda.TimeStampBlocco))
            {
                messaggioVideo = "Non è possibile eseguire lo sblocco della domanda. E' necessario contattare i referenti WebDom.";
                return false;
            }
            if (!SbloccaDomanda(numeroDomanda, datiSbloccoDomanda.MatricolaBlocco, datiSbloccoDomanda.TimeStampBlocco, out messaggioVideo))
                return false;
            return true;
        }

        public static string GetCodTipoRichiesta(DataSetDomanda dati)
        {
            string codTipoRichiesta = null;

            if (dati != null && dati.Istanza != null && dati.Istanza.Rows.Count > 0)
            {
                try
                {
                    codTipoRichiesta = dati.Istanza[0].CodTipoRichiesta;
                }
                catch (StrongTypingException)
                {
                    codTipoRichiesta = null;
                }

                if (codTipoRichiesta == string.Empty)
                {
                    if (dati.RequisitoParticolare != null && dati.RequisitoParticolare.Rows.Count > 0)
                    {
                        bool IsFiltroPresente = false;
                        for (int i = 0; i < dati.RequisitoParticolare.Rows.Count; i++)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(dati.RequisitoParticolare[i].Filtro) &&
                                    dati.RequisitoParticolare[i].Filtro.Trim().ToUpperInvariant() == "FFF")
                                {
                                    IsFiltroPresente = true;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                //Eccezione ignorata
                            }
                        }
                        if (!IsFiltroPresente)
                            codTipoRichiesta = null;
                    }
                    else
                        codTipoRichiesta = null;
                }

                List<string> filtriDaBonificarePerCumulo = new List<string> { "BNS", "BNX", "SCO" };
                if (Utility.IsDomandaCumulo(dati.Istanza[0].SiglaCatLav) && !string.IsNullOrEmpty(codTipoRichiesta) && codTipoRichiesta != "50" &&
                    dati.RequisitoParticolare != null && dati.RequisitoParticolare.Rows.Count > 0 &&
                    dati.RequisitoParticolare.Any(x => x["Filtro"] != DBNull.Value && filtriDaBonificarePerCumulo.Contains(x.Filtro)))
                    codTipoRichiesta = "50";
            }

            return codTipoRichiesta;
        }

        public static bool ControlloAttivitaWebDom(ServiceReferences.WebDom.DatiDomanda datiDomanda,
            GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiDomanda != null && datiDomanda.Dati != null &&
                datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows != null &&
                datiDomanda.Dati.Attivita.Rows.Count > 0)
            {

                for (int i = 0; i < datiDomanda.Dati.Attivita.Rows.Count; i++)
                {
                    DataSetDomanda.AttivitaRow attivita = datiDomanda.Dati.Attivita[i];
                    if (attivita != null)
                    {
                        string codAttivita = GetCodiceAttivita(datiDomanda, i);
                        if (int.Parse(codAttivita) == (int)CodiceAttivita.ChiarimentiNormativi ||
                            int.Parse(codAttivita) == (int)CodiceAttivita.DecisioneDirigente ||
                                int.Parse(codAttivita) == (int)CodiceAttivita.DirezioneTerritorialeLavoro ||
                                int.Parse(codAttivita) == (int)CodiceAttivita.UfficioSanitario)
                        {
                            if ((attivita["DataInizio"] != DBNull.Value && !string.IsNullOrEmpty(attivita["DataInizio"].ToString()))
                                    && (attivita["DataFine"] == DBNull.Value || string.IsNullOrEmpty(attivita["DataFine"].ToString())))
                            {
                                messaggioVideo = "Domanda non lavorabile. E' presente l'attività '" + codAttivita + "' aperta e non chiusa";
                                return false;
                            }
                        }
                    }
                }

                DataSetDomanda.AttivitaRow rigaUltimaAttivita = datiDomanda.Dati.Attivita[datiDomanda.Dati.Attivita.Rows.Count - 1];
                if (rigaUltimaAttivita != null)
                {
                    string codAttivitaPrec = GetCodiceAttivita(datiDomanda, datiDomanda.Dati.Attivita.Rows.Count - 1);
                    if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.InAcquisizione ||
                        int.Parse(codAttivitaPrec) == (int)CodiceAttivita.AttesaCalcolo ||
                        int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloErrato)
                    {
                        if ((rigaUltimaAttivita["DataInizio"] != DBNull.Value && !string.IsNullOrEmpty(rigaUltimaAttivita["DataInizio"].ToString()))
                            && (rigaUltimaAttivita["DataFine"] == DBNull.Value || string.IsNullOrEmpty(rigaUltimaAttivita["DataFine"].ToString())))
                        {
                            if (rigaUltimaAttivita["CodTipoProvvedimento"] == DBNull.Value ||
                                string.IsNullOrEmpty(rigaUltimaAttivita["CodTipoProvvedimento"].ToString()) ||
                                rigaUltimaAttivita["CodTipoProvvedimento"].ToString() != "017")
                            {
                                messaggioVideo = "Domanda non lavorabile. E' presente l'attività '" + codAttivitaPrec + "' aperta e non chiusa";
                                return false;
                            }
                            else
                            {
                                GestioneWebDom.ChiusuraUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, out messaggioVideo);
                                if (!string.IsNullOrEmpty(messaggioVideo))
                                    return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static bool AggiornaFondoWebDom(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, string codFondo, out string errori)
        {
            errori = string.Empty;

            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                return false;

            if (datiDomanda != null)
            {
                if (!AggiornaGestioneFondoEnte(datiPensione, matricolaOperatore, codFondo, datiDomanda.TimestampBlocco, out errori))
                    return false;
            }

            return true;
        }

        public static bool GetUltimaAttivita(long numDomanda, out CodiceAttivita? codAttivita, out string dataFineAttivita, out string errori)
        {
            codAttivita = null;
            dataFineAttivita = string.Empty;
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            GestioneWebDom.GetDomandaPerDomus(numDomanda.ToString(), out datiDomanda, out errori);

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                int indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 1;
                string codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                string dataFineAttivitaPrec = string.Empty;

                if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CollegamentoConOrganismoEstero)
                {
                    indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 2;
                    codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                }

                try
                {
                    dataFineAttivitaPrec = datiDomanda.Dati.Attivita[indexAttivitaPrec]["DataFine"] != DBNull.Value ?
                        datiDomanda.Dati.Attivita[indexAttivitaPrec].DataFine : string.Empty;
                }
                catch (Exception)
                {
                    //Eccezione ignorata
                }

                codAttivita = (CodiceAttivita)Enum.Parse(typeof(CodiceAttivita), codAttivitaPrec);
                dataFineAttivita = dataFineAttivitaPrec;
            }
            else
                return false;

            return true;
        }

        public static bool GetSedeDestinazione(long numeroDomanda, string codCatastale, string cap, out string sedeDestinazione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            sedeDestinazione = null;

            if (!GetSedeDestinazioneByResidenza(numeroDomanda, codCatastale, cap, out sedeDestinazione, out messaggioVideo))
                return false;

            return true;
        }

        public static bool GetCodUnitaProcesso(short? codiceSedeDestinazione, byte? centroOperativoDestinazione, string gestione, out byte? codUnitaProcesso, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            codUnitaProcesso = null;

            if (!GetCodUnitaProcessoBySedeDestinazione(codiceSedeDestinazione, centroOperativoDestinazione, gestione, out codUnitaProcesso, out messaggioVideo))
                return false;

            return true;
        }

        public static bool GetCodUnitaProcessoGP1ALZ6(short? codiceSedeDestinazione, byte? centroOperativoDestinazione, string gestione, out byte? codUnitaProcessoGP1ALZ6, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            codUnitaProcessoGP1ALZ6 = null;

            if (!GetCodUnitaProcessoBySedeGP1ALZ6(codiceSedeDestinazione, centroOperativoDestinazione, gestione, out codUnitaProcessoGP1ALZ6, out messaggioVideo))
                return false;

            return true;
        }

        public static bool VerificaFaseAttivitaPerAggiornamentoStazioneLavoro(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, out string errori)
        {
            errori = string.Empty;
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;

            //Rimuovo eventuali blocchi precedenti sulla domanda
            if (!SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                return false;

            //effettuo la get dei dati domanda
            if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                return false;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                int indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 1;
                string codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);

                //Se l'ultima attività è diversa da Attesa calcolo(ovvero che risulta calcolata solo al Calcolo, 
                //senza aver inizializzato lo stato necessario per aggiornare stazione di lavoro su webdom) non effettuo l'aggiornamento delle attività 
                //(ovvero la chiusura dell'attività In Liquidazione e l'apertura dell'attività Attesa calcolo)
                if (int.Parse(codAttivitaPrec) != (int)CodiceAttivita.AttesaCalcolo)
                {
                    AggiornamentoFaseAttivita(datiPensione, matricolaOperatore, sedeOperatore, out errori, datiDomanda);
                    if (!string.IsNullOrEmpty(errori))
                        return false;
                }
                else
                {
                    if (!SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                        return false;
                }
            }
            else
            {
                errori = "Controlli per WebDom: nessuna attività disponibile. La richiesta di aggiornamento di Fase e Attivita non è ammessa";
                return false;
            }

            return true;
        }
        #endregion public members

        #region private members
        private static string GetCodiceAttivita(ServiceReferences.WebDom.DatiDomanda datiDomanda, int index)
        {
            string codAttivitaPrec = "0";
            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                if (datiDomanda.Dati.Attivita[index]["CodAttivita"] != DBNull.Value && datiDomanda.Dati.Attivita[index]["CodAttivita"] != null)
                {
                    if (!string.IsNullOrEmpty(datiDomanda.Dati.Attivita[index]["CodAttivita"].ToString().Trim()))
                    {
                        codAttivitaPrec = datiDomanda.Dati.Attivita[index].CodAttivita;
                    }
                }
            }
            return codAttivitaPrec;
        }

        private static void VerificaBypassAggiornamentoFaseAttivita(ServiceReferences.WebDom.DatiDomanda datiDomanda, ref string errori)
        {
            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null &&
                datiDomanda.Dati.Attivita.Count > 0)
            {
                string attPrec = GetCodiceAttivita(datiDomanda, datiDomanda.Dati.Attivita.Rows.Count - 1);
                if (int.Parse(attPrec) == (int)CodiceAttivita.CalcoloEsatto ||
                    int.Parse(attPrec) == (int)CodiceAttivita.CalcoloProvvisorio ||
                    int.Parse(attPrec) == (int)CodiceAttivita.CalcoloProvvisorioEMENS ||
                    int.Parse(attPrec) == (int)CodiceAttivita.CalcoloProvvisorioDMAG)
                    errori = string.Empty;
            }
            return;
        }

        private static bool ChiudiIterCI(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, out string errori)
        {
            errori = string.Empty;

            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                return false;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                bool IsPresenteIterCI = false;
                for (int i = datiDomanda.Dati.Attivita.Rows.Count - 1; i >= 0; i--)
                {
                    string codAttivitaCI = GetCodiceAttivita(datiDomanda, i);
                    if (int.Parse(codAttivitaCI) == (int)CodiceAttivita.AvvioIterCI &&
                        datiDomanda.Dati.Attivita[i]["DataFine"] == DBNull.Value)
                    {
                        IsPresenteIterCI = true;
                        break;
                    }
                }
                if (IsPresenteIterCI)
                {
                    if (!FineAttivita(datiPensione, matricolaOperatore, CodiceAttivita.AvvioIterCI, datiDomanda, out errori))
                        return false;
                }
            }
            return true;
        }

        private static bool GetDomandaWithLockPerDomus(long numDomanda, string matricolaOperatore, short sedeOperatore, out ServiceReferences.WebDom.DatiDomanda datiDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            WSWebDomSoapClient proxy = null;
            datiDomanda = new DatiDomanda();
            string stackTrace = null;

            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (numDomanda == 0)
                    {
                        errori = "Errore durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }
                    proxy = new WSWebDomSoapClient();
                    datiDomanda = proxy.GetDomandaLock(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "",
                        numDomanda.ToString(), "S", matricolaOperatore,
                        sedeOperatore.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0'),
                        sedeOperatore.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0'), ref errori);
                    if (!String.IsNullOrEmpty(errori))
                    {
                        string extraErrore = string.Empty;
                        if (errori.StartsWith("0004"))
                            extraErrore = ". Sbloccare prima la domanda con la procedura RICH-DOM";
                        errori = "Errore durante il recupero delle informazioni della domanda: " + errori;
                        if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null &&
                            datiDomanda.Dati.Attivita.Count > 0)
                        {
                            string attPrec = GetCodiceAttivita(datiDomanda, datiDomanda.Dati.Attivita.Rows.Count - 1);
                            bool IsChiusa = false;
                            try
                            {
                                if (datiDomanda.Dati.Attivita[datiDomanda.Dati.Attivita.Count - 1]["DataFine"] != DBNull.Value &&
                                    !string.IsNullOrEmpty(datiDomanda.Dati.Attivita[datiDomanda.Dati.Attivita.Count - 1]["DataFine"].ToString()))
                                    IsChiusa = true;
                            }
                            catch (Exception)
                            {
                                //Eccezione ignorata
                            }
                            errori += " Ultima attività: (" + attPrec + ") nello stato di " +
                                (IsChiusa ? "chiusura" : "apertura") + (!string.IsNullOrEmpty(extraErrore) ? extraErrore : string.Empty);
                        }
                        return false;
                    }

                    if (datiDomanda != null)
                    {
                        GestioneSbloccoDomanda.DatiSbloccoDomanda datiSbloccoDomanda = new GestioneSbloccoDomanda.DatiSbloccoDomanda();
                        datiSbloccoDomanda.NDomus = numDomanda;
                        datiSbloccoDomanda.MatricolaBlocco = matricolaOperatore;
                        datiSbloccoDomanda.TimeStampBlocco = datiDomanda.TimestampBlocco;
                        GestioneSbloccoDomanda.SalvaSbloccoDomanda(datiSbloccoDomanda);
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    errori = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda per il blocco delle attività";
                        GestioneLogGenerico.SalvaLogGenerico(numDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool InizioAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, CodiceAttivita codAttivita, DatiDomanda datiDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = "";
            WSWebDomSoapClient proxy = null;
            AreaAgg areaAgg = new AreaAgg();
            string stackTrace = null;

            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (datiPensione.NDomus == 0)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }

                    areaAgg.CodiceAttivita = Utility.GetDescription(codAttivita);
                    areaAgg.DataInizio = Utility.GetDataElaborazionePensione(datiPensione).ToString("yyyyMMdd");
                    areaAgg.MatricolaCaricamento = matricolaOperatore;
                    areaAgg.NomeProcedura = ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "";
                    areaAgg.NumeroDomanda = datiPensione.NDomus.ToString();
                    //string timeStampBlocco = String.Format("{0}-{1}.000000", datiDomanda.TimestampBlocco.Substring(0, 10), datiDomanda.TimestampBlocco.Substring(11));
                    areaAgg.TimeStamp = datiDomanda.TimestampBlocco;

                    proxy = new WSWebDomSoapClient();
                    proxy.InizioAttivita(ref identity, ref areaAgg, ref errori);
                    if (!String.IsNullOrEmpty(errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante l'apertura dell'attività";
                        string parametri = null;
                        try { parametri = Utility.GetXmlFromObject(areaAgg); }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool FineAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, CodiceAttivita codAttivita, DatiDomanda datiDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WSWebDomSoapClient proxy = null;
            AreaAgg areaAgg = new AreaAgg();
            string stackTrace = null;

            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (datiPensione.NDomus == 0)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }

                    areaAgg.CodiceAttivita = Utility.GetDescription(codAttivita);
                    areaAgg.DataFine = Utility.GetDataElaborazionePensione(datiPensione).ToString("yyyyMMdd");
                    areaAgg.MatricolaCaricamento = matricolaOperatore;
                    areaAgg.NomeProcedura = ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "";
                    areaAgg.NumeroDomanda = datiPensione.NDomus.ToString();
                    //string timeStampBlocco = String.Format("{0}-{1}.000000", datiDomanda.TimestampBlocco.Substring(0, 10), datiDomanda.TimestampBlocco.Substring(11));
                    areaAgg.TimeStamp = datiDomanda.TimestampBlocco;

                    proxy = new WSWebDomSoapClient();
                    proxy.FineAttivita(ref identity, ref areaAgg, ref errori);
                    if (!String.IsNullOrEmpty(errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la chiusura dell'attività";
                        string parametri = null;
                        try { parametri = Utility.GetXmlFromObject(areaAgg); }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool FineAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, string codAttivita, DatiDomanda datiDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WSWebDomSoapClient proxy = null;
            AreaAgg areaAgg = new AreaAgg();

            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (datiPensione.NDomus == 0)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }

                    areaAgg.CodiceAttivita = codAttivita.PadLeft(5, '0');
                    areaAgg.DataFine = Utility.GetDataElaborazionePensione(datiPensione).ToString("yyyyMMdd");
                    areaAgg.MatricolaCaricamento = matricolaOperatore;
                    areaAgg.NomeProcedura = ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "";
                    areaAgg.NumeroDomanda = datiPensione.NDomus.ToString();
                    //string timeStampBlocco = String.Format("{0}-{1}.000000", datiDomanda.TimestampBlocco.Substring(0, 10), datiDomanda.TimestampBlocco.Substring(11));
                    areaAgg.TimeStamp = datiDomanda.TimestampBlocco;

                    proxy = new WSWebDomSoapClient();
                    proxy.FineAttivita(ref identity, ref areaAgg, ref errori);
                    if (!String.IsNullOrEmpty(errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la chiusura dell'attività";
                        string parametri = null;
                        try { parametri = Utility.GetXmlFromObject(areaAgg); }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool AggiornaFaseAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, CodiceAttivita codAttivita, DatiDomanda datiDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WSWebDomSoapClient proxy = null;
            AreaAgg areaAgg = new AreaAgg();

            Guid guid = Guid.NewGuid();
            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (datiPensione == null || datiPensione.NDomus == 0)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }

                    areaAgg.CodiceAttivita = Utility.GetDescription(codAttivita);
                    areaAgg.DataInizio = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.ToString("yyyyMMdd") : DateTime.Now.ToString("yyyyMMdd");
                    areaAgg.DataFine = datiPensione.DataElaborazione.HasValue ? datiPensione.DataElaborazione.Value.ToString("yyyyMMdd") : DateTime.Now.ToString("yyyyMMdd");
                    areaAgg.MatricolaCaricamento = matricolaOperatore;
                    areaAgg.NomeProcedura = ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "";
                    areaAgg.NumeroDomanda = datiPensione.NDomus.ToString();
                    //string timeStampBlocco = String.Format("{0}-{1}.000000", datiDomanda.TimestampBlocco.Substring(0, 10), datiDomanda.TimestampBlocco.Substring(11));
                    areaAgg.TimeStamp = datiDomanda.TimestampBlocco;
                    areaAgg.fase = new Fase();
                    areaAgg.fase.sCodSituazione = "0002";
                    areaAgg.fase.sDataSituazione = DateTime.Now.ToString("yyyyMMdd");

                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione || Utility.IsRiaperturaDomanda(datiPensione.Id))
                    {
                        Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                        areaAgg.pensioneGenerata = GetPensioneGenerata(datiDomanda);
                        if (tipoAppartenenza == Utility.TipoAppartenenza.AGO)
                        {
                            areaAgg.pensioneGenerata.sCodiceCertificatoPensioneGenerata = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "";
                            areaAgg.pensioneGenerata.sCodiceProvinciaPensioneGenerata = datiPensione.CodiceSedeDestinazione.HasValue ?
                                datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0') :
                                datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0');
                            areaAgg.pensioneGenerata.sCodiceZonaPensioneGenerata = datiPensione.CodiceSedeDestinazione.HasValue ?
                                datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0') :
                                datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0');
                        }
                        areaAgg.pensioneGenerata.sCodiceCentroOperativoPensioneGenerata = datiPensione.CodiceSedeDestinazione.HasValue ?
                                datiPensione.CentroOperativoDestinazione.GetValueOrDefault().ToString().PadLeft(2, '0').PadLeft(3, '0') : null;
                        areaAgg.pensioneRiferimento = GetPensioneRiferimento(datiDomanda);

                        GestioneControlliDinamici.ControlloDinamico ctrl = null;

                        if (tipoAppartenenza == Utility.TipoAppartenenza.AGO)
                            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaMemo114/2022AGO", out ctrl);
                        else if (tipoAppartenenza == Utility.TipoAppartenenza.FS)
                            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaMemo114/2022FS", out ctrl);
                        else if (tipoAppartenenza == Utility.TipoAppartenenza.CI)
                            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaMemo114/2022CI", out ctrl);

                        bool eseguiControllo = true;
                        if (tipoAppartenenza == Utility.TipoAppartenenza.CI)
                        {
                            List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
                            GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

                            if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0 && listaPrestazioniEstere[0].CodiceConvenzione != 12)
                                eseguiControllo = false;
                        }

                        if (ctrl != null && ctrl.ValoreControllo == "SI")
                        {
                            string categoria = "";
                            categoria = datiPensione.SiglaCategoria.Trim().Substring(2);
                            string codCat = datiPensione.GetCodCategoria();
                            List<string> lCategorieInpdap = new List<string> { "CPDEL", "CPS", "CPI", "CPUG" };
                            //Eng - escludere sCodTipoProvvedimento = "014" anche per VOCTPS (0213), IOCTPS (0214), SOCTPS (0215) 
                            List<string> lCodCatBypassInvioMailDirettore = new List<string> { "0010", "0011", "0012", "0013", "0014", "0030", "0031", "0035", "0036", "0037", "0040", "0043", 
                                "0044", "0070", "0071", "0072", "0076", "0077", "0078", "0170", "0171", "0172", "0032", "0033", "034", "0027", "0028", "0029 ", "0127", "0128", "0129", "0198", "0199", "0200", "0213", "0214", "0215" };
                            BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                            BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                            //ENG - RIC REVERSIBILITA 024: implementazione flusso per riconoscere le reversibilità "vecchie" 
                            GestioneLavorazione.DatiLavorazione datiLavorazione = null;
                            GestioneLavorazione.GetLavorazioneByIdPensione(datiPensione.Id, out datiLavorazione);

                            if (!Utility.IsDomandaENPALS(datiPensione.Gestione) && (Utility.IsRicostituzione_MotiviContributivi(datiPensione) || Utility.IsRiaperturaDomanda(datiPensione.Id)) && !(datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Fase != null && datiDomanda.Dati.Fase.Count > 0 &&
                               (datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0033" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0034" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0035" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0036" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0038")) &&
                               !lCategorieInpdap.Exists(x => x == categoria) && !lCodCatBypassInvioMailDirettore.Contains(codCat.Trim()) && !Utility.IsDomandaReversibilitaOrRicostituzione(datiPensione, datiDanteCausa, datiLavorazione) && eseguiControllo &&
                                datiPensione.AttivitaEconomica != 52 && datiPensione.ProfessioneIndividuale != 777)
                            {
                                if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica)
                                {
                                    switch (codAttivita)
                                    {
                                        case CodiceAttivita.CalcoloEsatto:
                                        case CodiceAttivita.CalcoloProvvisorio:
                                        case CodiceAttivita.CalcoloProvvisorioDMAG:
                                        case CodiceAttivita.CalcoloProvvisorioEMENS:
                                            areaAgg.attivita = new Attivita();
                                            areaAgg.attivita.sCodTipoProvvedimento = "014";
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        areaAgg.pensioneGenerata = new PensioneGenerata();
                        Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
                        if (tipoAppartenenza.HasValue)
                        {
                            switch (tipoAppartenenza.Value)
                            {
                                case Utility.TipoAppartenenza.FS:
                                    Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(tipoAppartenenza, datiPensione.SiglaCategoria);
                                    if (tipoFondo != null && (tipoFondo.ToString() == "PT" || tipoFondo.ToString() == "PI"))
                                        areaAgg.pensioneGenerata.sSiglaCategoriaPensioneGenerata = datiPensione.SiglaCategoria.PadRight(8, ' ');
                                    else if (Utility.IsDomandaINPDAP(datiPensione.Gestione))
                                        areaAgg.pensioneGenerata.sSiglaCategoriaPensioneGenerata = datiPensione.SiglaCategoria.PadRight(8, ' ');
                                    else
                                        areaAgg.pensioneGenerata.sSiglaCategoriaPensioneGenerata = tipoFondo.ToString().PadRight(8, ' ');
                                    break;
                                case Utility.TipoAppartenenza.CI:
                                case Utility.TipoAppartenenza.AGO:
                                    areaAgg.pensioneGenerata.sSiglaCategoriaPensioneGenerata = datiPensione.SiglaCategoria.PadRight(8, ' ');

                                    bool eseguiControllo = true;
                                    if (tipoAppartenenza == Utility.TipoAppartenenza.CI)
                                    {
                                        List<GestioneDatiContributiviCi.PensioniCiPrestazioniEE> listaPrestazioniEstere = null;
                                        GestioneDatiContributiviCi.GetPrestazioniEEByIdPensione(datiPensione.Id, out listaPrestazioniEstere);

                                        if (listaPrestazioniEstere != null && listaPrestazioniEstere.Count() > 0 && listaPrestazioniEstere[0].CodiceConvenzione != 12)
                                            eseguiControllo = false;
                                    }

                                    //Codice Provvedimento per invio mail per domande PL AGO NO UNICARPE
                                    if (Utility.IsDomandaUnicarpe(datiPensione, true) != Utility.TipoUnicarpe.Automatica && eseguiControllo)
                                    {
                                        switch (codAttivita)
                                        {
                                            case CodiceAttivita.CalcoloEsatto:
                                            case CodiceAttivita.CalcoloProvvisorio:
                                            case CodiceAttivita.CalcoloProvvisorioDMAG:
                                            case CodiceAttivita.CalcoloProvvisorioEMENS:
                                                if (GestioneDecodifica.IsBypassInvioMailDirettore(datiPensione.SiglaCategoria) || Utility.IsDomandaReversibilita(datiPensione) || (datiPensione.AttivitaEconomica == 52 && datiPensione.ProfessioneIndividuale == 777))
                                                    break;
                                                areaAgg.attivita = new Attivita();
                                                areaAgg.attivita.sCodTipoProvvedimento = "014";
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                            areaAgg.pensioneGenerata.sSiglaCategoriaPensioneGenerata = datiPensione.SiglaCategoria.PadRight(8, ' ');
                        areaAgg.pensioneGenerata.sCodiceCertificatoPensioneGenerata = datiPensione.NCertificato.HasValue ? datiPensione.NCertificato.Value.ToString().PadLeft(8, '0') : "";
                        if (tipoAppartenenza != Utility.TipoAppartenenza.CI)
                        {
                            areaAgg.pensioneGenerata.sCodiceProvinciaPensioneGenerata = datiPensione.CodiceSedeDestinazione.HasValue ?
                                datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0') :
                                datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0');
                            areaAgg.pensioneGenerata.sCodiceZonaPensioneGenerata = datiPensione.CodiceSedeDestinazione.HasValue ?
                                datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0') :
                                datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0');
                            areaAgg.pensioneGenerata.sCodiceCentroOperativoPensioneGenerata = datiPensione.CodiceSedeDestinazione.HasValue ?
                                    datiPensione.CentroOperativoDestinazione.GetValueOrDefault().ToString().PadLeft(2, '0').PadLeft(3, '0') : null;
                        }
                        else
                        {
                            areaAgg.pensioneGenerata.sCodiceProvinciaPensioneGenerata = (datiPensione.CodiceSedeDestinazione.HasValue && !Utility.IsDomandaAPEPrecoci(datiPensione)) ?
                                datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0') :
                                datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0');
                            areaAgg.pensioneGenerata.sCodiceZonaPensioneGenerata = (datiPensione.CodiceSedeDestinazione.HasValue && !Utility.IsDomandaAPEPrecoci(datiPensione)) ?
                                datiPensione.CodiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0') :
                                datiPensione.CodiceSede.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0');
                            areaAgg.pensioneGenerata.sCodiceCentroOperativoPensioneGenerata = (datiPensione.CodiceSedeDestinazione.HasValue && !Utility.IsDomandaAPEPrecoci(datiPensione)) ?
                                    datiPensione.CentroOperativoDestinazione.GetValueOrDefault().ToString().PadLeft(2, '0').PadLeft(3, '0') : null;

                        }

                        try
                        {
                            string catNum = datiPensione.GetCodCategoria();
                            areaAgg.pensioneGenerata.sCodiceCategoriaPensioneGenerata = catNum;

                            if (IsPensioneRiferimentoRequired(datiPensione))
                                areaAgg.pensioneRiferimento = GetPensioneRiferimento(datiDomanda);
                        }
                        catch (Exception Ex)
                        {
                            errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + Ex.Message;
                            INPS.DNA.Logging.Logger.LogException(Ex);
                            return false;
                        }
                    }
                    areaAgg.pensioneGenerata.sDataDecorrenzaPensioneGenerata = datiPensione.DecorrenzaOriginaria.HasValue ? datiPensione.DecorrenzaOriginaria.Value.ToString("yyyyMMdd") : "";

                    proxy = new WSWebDomSoapClient();
                    GestioneLogSoap.SalvaLogSoap(areaAgg, Utility.Servizio.SrvWebDom, Utility.MetodoServizio.AggiornaFaseAttivita, Utility.SOAPLogDirection.IN, datiPensione.NDomus.ToString(), guid);
                    proxy.AggiornaFaseAttivita(ref identity, ref areaAgg, ref errori);
                    GestioneLogSoap.SalvaLogSoap(areaAgg, Utility.Servizio.SrvWebDom, Utility.MetodoServizio.AggiornaFaseAttivita, Utility.SOAPLogDirection.OUT, datiPensione.NDomus.ToString(), guid);
                    if (!String.IsNullOrEmpty(errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante l'aggiornamento finale delle attività della domanda";
                        string parametri = null;
                        try { parametri = Utility.GetXmlFromObject(areaAgg); }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool IsPensioneRiferimentoRequired(GestionePensione.DatiPensione datiPensione)
        {
            bool ret = GestioneDecodifica.IsPensioneRiferimentoObbligatoria(datiPensione.Gruppo, datiPensione.Prodotto, datiPensione.Tipo);
            // //Trasformazioni AOI
            //(datiPensione.Gruppo == "0001" && (datiPensione.Prodotto == "0001" || datiPensione.Prodotto == "0002") && datiPensione.Tipo == "0002")
            ////0001-0002-0003 Pensione di vecchiaia a seguito di trasformazione Pensione di invalidità
            ////0001-0002-0004 Pensione di vecchiaia in base all'art. 2/ter L. 114/74
            // || (datiPensione.Gruppo == "0001" && datiPensione.Prodotto == "0002" && (datiPensione.Tipo == "0003" || datiPensione.Tipo == "0004"))
            ////0002-0011-0004 Assegno ordinario di invalidità in base all'art.2/ter L.114/74
            // || (datiPensione.Gruppo == "0002" && datiPensione.Prodotto == "0011" && datiPensione.Tipo == "0004")
            ////0061-0123-0001 Conferma assegno di invalidità
            // || (datiPensione.Gruppo == "0061" && datiPensione.Prodotto == "0123" && datiPensione.Tipo == "0001")
            ////0061-0124-0126 Revisione a richiesta per motivi sanitari
            // || (datiPensione.Gruppo == "0061" && datiPensione.Prodotto == "0124" && datiPensione.Tipo == "0126");
            return ret;
        }

        private static bool SbloccaDomanda(long numeroDomanda, string matricolaBlocco, string timeStampBlocco, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WSWebDomSoapClient proxy = null;
            AreaAgg areaAgg = new AreaAgg();

            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (numeroDomanda == 0)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }

                    areaAgg.MatricolaCaricamento = matricolaBlocco;
                    areaAgg.NomeProcedura = ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "";
                    areaAgg.NumeroDomanda = numeroDomanda.ToString();
                    areaAgg.TimeStamp = timeStampBlocco;

                    proxy = new WSWebDomSoapClient();
                    proxy.SbloccaDomanda(ref identity, areaAgg, ref errori);
                    if (!String.IsNullOrEmpty(errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda per lo sblocco delle attività";
                        string parametri = null;
                        try { parametri = Utility.GetXmlFromObject(areaAgg); }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool VerificaCompatibilitaAperturaAttivita(CodiceAttivita codAttivita, string codAttivitaPrec, out string errori)
        {
            errori = string.Empty;

            try
            {
                switch (codAttivita)
                {
                    case CodiceAttivita.Prelievo:
                        errori = "Controlli per WebDom: Non è possibile inserire l'attività '" + codAttivita.ToString() +
                                "' (" + Utility.GetDescription(codAttivita) + ") " +
                            "in presenza della precedente attività (" + codAttivitaPrec + ")";
                        return false;

                    case CodiceAttivita.InAcquisizione:
                        if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloEsatto) //|| int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloErrato) //problemi su eliminazione di una domanda calcolata errata da ReEng
                        {
                            errori = "Controlli per WebDom: Non è possibile inserire l'attività '" + codAttivita.ToString() +
                                "' (" + Utility.GetDescription(codAttivita) + ") " +
                            "in presenza della precedente attività (" + codAttivitaPrec + ")";
                            return false;
                        }
                        break;

                    case CodiceAttivita.AttesaCalcolo:
                        if (int.Parse(codAttivitaPrec) != (int)CodiceAttivita.Prelievo && int.Parse(codAttivitaPrec) != (int)CodiceAttivita.InAcquisizione
                            && int.Parse(codAttivitaPrec) != (int)CodiceAttivita.CalcoloErrato && int.Parse(codAttivitaPrec) != 0)
                        {
                            errori = "Controlli per WebDom: Non è possibile inserire l'attività '" + codAttivita.ToString() +
                                "' (" + Utility.GetDescription(codAttivita) + ") " +
                            "in presenza della precedente attività (" + codAttivitaPrec + ")";
                            return false;
                        }
                        break;
                    case CodiceAttivita.CalcoloEsatto:
                    case CodiceAttivita.CalcoloProvvisorio:
                    case CodiceAttivita.CalcoloProvvisorioEMENS:
                    case CodiceAttivita.CalcoloProvvisorioDMAG:
                    case CodiceAttivita.CalcoloErrato:
                        if (int.Parse(codAttivitaPrec) != (int)CodiceAttivita.AttesaCalcolo)
                        {
                            errori = "Controlli per WebDom: Non è possibile inserire l'attività '" + codAttivita.ToString() +
                                "' (" + Utility.GetDescription(codAttivita) + ") " +
                            "in presenza della precedente attività (" + codAttivitaPrec + ")";
                            return false;
                        }
                        break;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool VerificaCompatibilitaChiusuraAttivita(CodiceAttivita codAttivita, string codAttivitaPrec, out string errori)
        {
            errori = string.Empty;

            try
            {
                if ((int)codAttivita != int.Parse(codAttivitaPrec))
                {
                    errori = "Controlli per WebDom: L' attività che si sta tentando di chiudere '" + codAttivita.ToString() + "' (" + Utility.GetDescription(codAttivita) +
                        ") non corrisponde all'ultima attività aperta e non chiusa (" + codAttivitaPrec + ")";
                    return false;
                }

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool VerificaCompatibilitaAggiornamentoFaseAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore,
           ref ServiceReferences.WebDom.DatiDomanda datiDomanda, out bool bypassAggiornamento, out string errori)
        {
            errori = string.Empty;
            bypassAggiornamento = false;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                int indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 1;

                string codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloEsatto ||
                    int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorio ||
                    int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioEMENS ||
                    int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioDMAG)
                {
                    if (!SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                        return false;
                    bypassAggiornamento = true;
                    return true;
                }

                //RIC CI MANUALI E TRF SIA MANUALI CHE AUTOMATICHE:
                //controllo che le due attività, "Avvio Iter CI" e "Collegamento con Organismo Estero", siano chiuse per poter considerare l'attività di "Attesa Calcolo" e poter successivamente
                //impostare l'attività "Calcolo Esatto"
                if (((Utility.IsRicostituzione(datiPensione.Gruppo) && !Utility.IsDomandaAutomatica(datiPensione)) || (datiDomanda.Dati != null && datiDomanda.Dati.Fase != null && datiDomanda.Dati.Fase.Count > 0 &&
                    (datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0060" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0062" || datiDomanda.Dati.Fase.LastOrDefault().CodFase == "0063")))
                     && Utility.GetTipoAppartenenza(datiPensione.IndConvInt.GetValueOrDefault(), datiPensione.Gestione) == Utility.TipoAppartenenza.CI)
                {
                    if (Int16.Parse(codAttivitaPrec) == (int)CodiceAttivita.AvvioIterCI)
                    {
                        string dataChiusuraAttivitaPrec = datiDomanda.Dati.Attivita[indexAttivitaPrec]["DataFine"] != DBNull.Value ? datiDomanda.Dati.Attivita[indexAttivitaPrec].DataFine : string.Empty;
                        if (!String.IsNullOrEmpty(dataChiusuraAttivitaPrec))
                        {
                            indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 2;
                            codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                            if (Int16.Parse(codAttivitaPrec) == (int)CodiceAttivita.CollegamentoConOrganismoEstero)
                            {
                                dataChiusuraAttivitaPrec = datiDomanda.Dati.Attivita[indexAttivitaPrec]["DataFine"] != DBNull.Value ? datiDomanda.Dati.Attivita[indexAttivitaPrec].DataFine : string.Empty;
                                if (!String.IsNullOrEmpty(dataChiusuraAttivitaPrec))
                                {
                                    indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 3; //Attivita "Attesa Calcolo"
                                    codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                                    if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloEsatto ||
                                        int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorio ||
                                        int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioEMENS ||
                                        int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioDMAG)
                                    {
                                        if (!SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                                            return false;
                                        bypassAggiornamento = true;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    //ENG - aggiunta gestione per le domande che hanno l'attività Avvio iter dopo Istruttoria perchè presente uno o più stato/i estero/i su stazione lavoro
                    else if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CollegamentoConOrganismoEstero)
                    {
                        indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 2;
                        codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                        if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloEsatto ||
                            int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorio ||
                            int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioEMENS ||
                            int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioDMAG)
                        {
                            if (!SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                                return false;
                            bypassAggiornamento = true;
                            return true;
                        }
                    }
                }
                else
                {
                    if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CollegamentoConOrganismoEstero)
                    {
                        indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 2;
                        codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                        if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloEsatto ||
                            int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorio ||
                            int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioEMENS ||
                            int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CalcoloProvvisorioDMAG)
                        {
                            if (!SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                                return false;
                            bypassAggiornamento = true;
                            return true;
                        }
                    }
                }

                if (int.Parse(codAttivitaPrec) != (int)CodiceAttivita.AttesaCalcolo)
                {
                    bool IsAppenaChiusa = false;
                    if (!FineUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, datiDomanda, out IsAppenaChiusa, out errori))
                        return false;

                    if (IsAppenaChiusa)
                    {
                        AperturaAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.AttesaCalcolo, out errori);
                        if (!string.IsNullOrEmpty(errori))
                            return false;
                    }
                    else
                    {
                        if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CorrispondenzaConIlRichiedente)
                        {
                            //chiudo l'attivita In Liquidazione se risulta aperta
                            if (IsAttivitaAperta(datiDomanda, GestioneWebDom.CodiceAttivita.InAcquisizione))
                                if (!FineAttivita(datiPensione, matricolaOperatore, GestioneWebDom.CodiceAttivita.InAcquisizione, datiDomanda, out errori))
                                    return false;
                        }

                        if (!InizioAttivita(datiPensione, matricolaOperatore, GestioneWebDom.CodiceAttivita.AttesaCalcolo, datiDomanda, out errori))
                            return false;
                    }

                    datiDomanda = null;
                    if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                        return false;

                    //aggiorno l'indice dell'ultima attività post get datiDomanda da WEBDOM
                    indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 1;

                    if (Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione) == Utility.TipoAppartenenza.CI)
                    {
                        if (!SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                            return false;
                        bypassAggiornamento = true;
                    }
                }

                string dataFineAttivitaPrec = string.Empty;
                try
                {
                    dataFineAttivitaPrec = datiDomanda.Dati.Attivita[indexAttivitaPrec]["DataFine"] != DBNull.Value ?
                            datiDomanda.Dati.Attivita[indexAttivitaPrec].DataFine : string.Empty;
                }
                catch (Exception)
                {
                    //Eccezione ignorata
                }

                if (!string.IsNullOrEmpty(dataFineAttivitaPrec))
                {
                    if (!InizioAttivita(datiPensione, matricolaOperatore, GestioneWebDom.CodiceAttivita.AttesaCalcolo, datiDomanda, out errori))
                        return false;

                    datiDomanda = null;
                    if (!GetDomandaWithLockPerDomus(datiPensione.NDomus, matricolaOperatore, sedeOperatore, out datiDomanda, out errori))
                        return false;
                }
            }
            else
            {
                errori = "Controlli per WebDom: nessuna attività disponibile. La richiesta di aggiornamento di Fase e Attivita non è ammessa";
                return false;
            }

            return true;
        }

        private static bool FineUltimaAttivita(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, ServiceReferences.WebDom.DatiDomanda datiDomanda,
            out bool IsAppenaChiusa, out string errori)
        {
            errori = string.Empty;
            IsAppenaChiusa = false;

            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                int indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 1;
                string codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                string dataFineAttivitaPrec = string.Empty;

                if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.CollegamentoConOrganismoEstero)
                {
                    indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 2;
                    codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);
                }

                try
                {
                    dataFineAttivitaPrec = datiDomanda.Dati.Attivita[indexAttivitaPrec]["DataFine"] != DBNull.Value ?
                        datiDomanda.Dati.Attivita[indexAttivitaPrec].DataFine : string.Empty;
                }
                catch (Exception)
                {
                    //Eccezione ignorata
                }

                if (int.Parse(codAttivitaPrec) != 0) //0 = In Istruttoria
                {
                    if (int.Parse(codAttivitaPrec) != (int)CodiceAttivita.AvvioIterCI)
                    {
                        if (int.Parse(codAttivitaPrec) != (int)CodiceAttivita.RichiestaDocumenti && String.IsNullOrEmpty(dataFineAttivitaPrec))
                        {
                            if (!FineAttivita(datiPensione, matricolaOperatore, codAttivitaPrec, datiDomanda, out errori))
                                return false;
                            IsAppenaChiusa = true;
                        }
                        else if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.RichiestaDocumenti && datiDomanda.Dati.Attivita.Rows.Count > 1)
                        {
                            indexAttivitaPrec = datiDomanda.Dati.Attivita.Rows.Count - 2;
                            codAttivitaPrec = GetCodiceAttivita(datiDomanda, indexAttivitaPrec);

                            try
                            {
                                dataFineAttivitaPrec = datiDomanda.Dati.Attivita[indexAttivitaPrec]["DataFine"] != DBNull.Value ?
                                    datiDomanda.Dati.Attivita[indexAttivitaPrec].DataFine : string.Empty;
                            }
                            catch (Exception)
                            {
                                //Eccezione ignorata
                            }

                            if (int.Parse(codAttivitaPrec) != 0) //0 = In Istruttoria
                            {
                                if (int.Parse(codAttivitaPrec) == (int)CodiceAttivita.InAcquisizione)
                                {
                                    if (String.IsNullOrEmpty(dataFineAttivitaPrec))
                                    {
                                        if (!FineAttivita(datiPensione, matricolaOperatore, codAttivitaPrec, datiDomanda, out errori))
                                            return false;
                                        IsAppenaChiusa = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                return true;

            return true;
        }

        private static bool ControllaStatoPensionePerAggiornamento(GestionePensione.DatiPensione datiPensione)
        {
            if (datiPensione != null && datiPensione.StatoPensione.HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).HasValue &&
                Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value).Value == Utility.StatoPensione.CalcolataNoWebDom)
                return true;
            else
                return false;
        }

        private static PensioneGenerata GetPensioneGenerata(ServiceReferences.WebDom.DatiDomanda datiDomanda)
        {
            PensioneGenerata pensioneGenerata = new PensioneGenerata();
            if (datiDomanda.Dati.PensioneGenerata != null && datiDomanda.Dati.PensioneGenerata.Count > 0)
            {
                ServiceReferences.WebDom.DataSetDomanda.PensioneGenerataRow pG = datiDomanda.Dati.PensioneGenerata[0];
                pensioneGenerata.sCodiceCategoriaPensioneGenerata = pG["CodCatPensioneGen"] != DBNull.Value ? pG.CodCatPensioneGen : string.Empty;
                pensioneGenerata.sCodiceCertificatoPensioneGenerata = pG["NumCertificatoGen"] != DBNull.Value ? pG.NumCertificatoGen : string.Empty;
                pensioneGenerata.sCodiceProvinciaPensioneGenerata = pG["CodProvinciaGen"] != DBNull.Value ? pG.CodProvinciaGen : string.Empty;
                pensioneGenerata.sCodiceZonaPensioneGenerata = pG["CodZonaGen"] != DBNull.Value ? pG.CodZonaGen : string.Empty;
                pensioneGenerata.sDataDecorrenzaPensioneGenerata = pG["DataDecorrenzaGen"] != DBNull.Value ? pG.DataDecorrenzaGen : string.Empty;
                pensioneGenerata.sSiglaCategoriaPensioneGenerata = pG["SiglaCatLav"] != DBNull.Value ? pG.SiglaCatLav : string.Empty;
            }
            return pensioneGenerata;
        }

        private static PensioneRiferimento GetPensioneRiferimento(ServiceReferences.WebDom.DatiDomanda datiDomanda)
        {
            PensioneRiferimento pensioneRiferimento = new PensioneRiferimento();
            if (datiDomanda.Dati.PensioneRiferimento != null && datiDomanda.Dati.PensioneRiferimento.Count > 0)
            {
                ServiceReferences.WebDom.DataSetDomanda.PensioneRiferimentoRow pR = datiDomanda.Dati.PensioneRiferimento[0];
                pensioneRiferimento.sCodiceCategoriaPensioneRiferimento = pR["CodCatPensioneRif"] != DBNull.Value ? pR.CodCatPensioneRif : string.Empty;
                pensioneRiferimento.sCodiceCertificatoPensioneRiferimento = pR["NumCertificatoRif"] != DBNull.Value ? pR.NumCertificatoRif : string.Empty;
                pensioneRiferimento.sCodiceProvinciaPensioneRiferimento = pR["CodProvinciaRif"] != DBNull.Value ? pR.CodProvinciaRif : string.Empty;
                pensioneRiferimento.sCodiceZonaPensioneRiferimento = pR["CodZonaRif"] != DBNull.Value ? pR.CodZonaRif : string.Empty;
                pensioneRiferimento.sNomeCategoriaPensioneRiferimento = pR["SiglaCatLav"] != DBNull.Value ? pR.SiglaCatLav : string.Empty;

                //20150901 - Commentato perchè la sigla categoria arriva da Webdom nel formato corretto
                //if (tipoAppartenenza.GetValueOrDefault() == Utility.TipoAppartenenza.FS && pensioneRiferimento.sNomeCategoriaPensioneRiferimento.Length > 1)
                //    pensioneRiferimento.sNomeCategoriaPensioneRiferimento = pensioneRiferimento.sNomeCategoriaPensioneRiferimento.Substring(1);
            }
            return pensioneRiferimento;
        }

        public static CodiceAttivita GetAttivitaDiChiusura(GestionePensione.DatiPensione datiPensione)
        {
            CodiceAttivita attivitaDiChiusura = CodiceAttivita.CalcoloEsatto;

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
            GestioneNuoveLiquidate.NuoveLiquidate nuoveLiquidate = null;
            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;

            // L'attività provvisoria viene impostata solo per le domande PL (no superstiti) e per le superstiti non di reversibilità
            if ((tipoDomanda == Utility.TipoDomanda.Normale || tipoDomanda == Utility.TipoDomanda.Ripristino || tipoDomanda == Utility.TipoDomanda.Riliquidazione ||
                tipoDomanda == Utility.TipoDomanda.RipristinoSuperstiti || tipoDomanda == Utility.TipoDomanda.RiliquidazioneSuperstiti ||
                (tipoDomanda == Utility.TipoDomanda.Superstiti && !Utility.IsDomandaReversibilita(datiPensione))) && !isRiaperturaDomanda)
            {
                switch (tipoAppartenenza.GetValueOrDefault())
                {
                    case Utility.TipoAppartenenza.AGO:
                        GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);
                        if (nuoveLiquidate != null && nuoveLiquidate.FlagProvvisoria.GetValueOrDefault())
                        {
                            attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorio;
                        }
                        else
                        {
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria != null && !string.IsNullOrEmpty(datiIstruttoria.ModalitaLiquidazione))
                            {
                                switch (datiIstruttoria.ModalitaLiquidazione.Trim())
                                {
                                    case "1":
                                    case "2":
                                    case "3":
                                    case "4":
                                    case "5":
                                        attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorio;
                                        break;
                                    case "6":
                                    case "7":
                                    case "8":
                                    case "9":
                                    case "10":
                                    case "11":
                                        attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorioEMENS;
                                        break;
                                    case "20":
                                    case "21":
                                    case "22":
                                    case "23":
                                    case "24":
                                    case "25":
                                    case "26":
                                    case "27":
                                    case "28":
                                    case "29":
                                    case "30":
                                    case "31":
                                        attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorioDMAG;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    case Utility.TipoAppartenenza.FS:
                        GestioneControlliDinamici.ControlloDinamico ctrl = null;
                        GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("CalcoloProvvisorioFS", out ctrl);
                        if (ctrl != null && ctrl.ValoreControllo == "SI")
                        {
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria != null && datiIstruttoria.CodiceComunicazioneCampo3.HasValue)
                            {
                                switch (datiIstruttoria.CodiceComunicazioneCampo3.Value)
                                {
                                    case 'A':
                                    case 'B':
                                    case 'C':
                                    case 'M':
                                    case 'N':
                                    case 'P':
                                        attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorio;
                                        break;
                                    case 'D':
                                    case 'E':
                                    case 'F':
                                    case 'G':
                                    case 'H':
                                    case 'I':
                                        attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorioEMENS;
                                        break;
                                    case 'J':
                                    case 'K':
                                    case 'L':
                                    case 'O':
                                    case 'Q':
                                    case 'R':
                                    case 'S':
                                    case 'T':
                                    case 'U':
                                    case 'V':
                                    case 'Z':
                                        attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorioDMAG;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    case Utility.TipoAppartenenza.CI:
                        GestioneNuoveLiquidate.GetNuoveLiquidateByIdPensione(datiPensione.Id, out nuoveLiquidate);
                        if (nuoveLiquidate != null && nuoveLiquidate.FlagProvvisoria.GetValueOrDefault())
                        {
                            attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorio;
                        }
                        else
                        {
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria != null && !string.IsNullOrEmpty(datiIstruttoria.ModalitaLiquidazione))
                            {
                                attivitaDiChiusura = CodiceAttivita.CalcoloProvvisorio;
                                break;
                            }
                        }
                        break;
                }
            }

            return attivitaDiChiusura;
        }

        private static bool AggiornaGestioneFondoEnte(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, string codFondo, string timeStampBlocco, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;

            WSWebDomSoapClient proxy = null;
            DatiGestioneFondoEnte datiGestioneFondoEnte = new DatiGestioneFondoEnte();
            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (datiPensione == null || datiPensione.NDomus == 0)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }

                    proxy = new WSWebDomSoapClient();

                    datiGestioneFondoEnte.CodiceApplicazione = System.Configuration.ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? System.Configuration.ConfigurationManager.AppSettings["WEBDOM-CODE"] : "";
                    datiGestioneFondoEnte.Matricola = matricolaOperatore;
                    datiGestioneFondoEnte.Timestamp = timeStampBlocco;
                    datiGestioneFondoEnte.NumDomanda = datiPensione.NDomus.ToString();
                    datiGestioneFondoEnte.CodGestione = datiPensione.Gestione;
                    datiGestioneFondoEnte.CodFondo = codFondo;
                    datiGestioneFondoEnte.CodEnte = datiPensione.Ente;
                    datiGestioneFondoEnte.IndConvInt = datiPensione.IndConvInt.GetValueOrDefault() ? "1" : "0";

                    if (!proxy.AggiornaGestioneFondoEnte(ref identity, datiGestioneFondoEnte, ref errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
                        return false;
                    }
                    if (!string.IsNullOrEmpty(errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante la modifica di Gestione / Fondo / Ente";
                        string parametri = null;
                        try { parametri = Utility.GetXmlFromObject(datiGestioneFondoEnte); }
                        catch (Exception)
                        {
                            // Eccezione ignorata
                        }
                        GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        internal static bool GetDomandaPerDomus(string numDomanda, out DatiDomanda datiDomanda, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            WSWebDomSoapClient proxy = null;
            datiDomanda = new DatiDomanda();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (numDomanda == null || numDomanda.Trim() == String.Empty)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }
                    proxy = new WSWebDomSoapClient();
                    datiDomanda = proxy.GetDomanda(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "",
                        numDomanda, "S", ref errori);
                    if (!String.IsNullOrEmpty(errori))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + errori;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda";
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(datiDomanda, Utility.Servizio.SrvWebDom, Utility.MetodoServizio.GetDomanda, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        internal static bool GetDomandePerCodiceFiscale(string codiceFiscale, string codRelazioneSoggetto, out List<DatiDomanda> elencoDatiDomanda, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            StringBuilder messaggioVideoStrBuilder = new StringBuilder();
            elencoDatiDomanda = null;
            WSWebDomSoapClient proxy = null;
            RispostaCF rispostaCF = new RispostaCF();

            Identity identity = new Identity();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (codiceFiscale == null || codiceFiscale.Trim() == String.Empty)
                    {
                        messaggioVideo = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }
                    proxy = new WSWebDomSoapClient();
                    rispostaCF = proxy.GetDomandePerCodFiscRelSogg(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "", codiceFiscale, codRelazioneSoggetto);
                    if (rispostaCF == null || !rispostaCF.Esito)
                    {
                        if (rispostaCF != null)
                            messaggioVideoStrBuilder.Append(rispostaCF.DescrizioneErrore);
                        else
                            messaggioVideo = "Nessuna risposta durante il recupero delle domande";
                        return false;
                    }
                    if (rispostaCF.Domande != null && rispostaCF.Domande.Length > 0)
                    {
                        elencoDatiDomanda = new List<ServiceReferences.WebDom.DatiDomanda>();
                        foreach (ServiceReferences.WebDom.DomandaCF domandaCF in rispostaCF.Domande)
                        {
                            string errori = string.Empty;
                            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
                            bool bTest = GetDomandaPerDomus(domandaCF.NumDomanda, out datiDomanda, out errori);

                            if (bTest && String.IsNullOrEmpty(errori))
                            {
                                if (codRelazioneSoggetto != "DA")
                                    //Controllo domande lavorabili
                                    if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Fase != null && datiDomanda.Dati.Istanza != null)
                                    {
                                        string codTipoRichiesta = null;

                                        bool? bNull = null;
                                        bool? IndConvInt = !string.IsNullOrEmpty(datiDomanda.Dati.Istanza[0].IndConvInt) ? ((datiDomanda.Dati.Istanza[0].IndConvInt.Trim() == "0" || datiDomanda.Dati.Istanza[0].IndConvInt.Trim().ToLowerInvariant() == "false") ? false : true) : bNull;

                                        codTipoRichiesta = GestioneWebDom.GetCodTipoRichiesta(datiDomanda.Dati);
                                        Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(IndConvInt, datiDomanda.Dati.Istanza[0].CodGestione);
                                        short codiceSede = short.Parse(datiDomanda.Dati.Domanda[0].CodProvincia.Substring(1, 2) + datiDomanda.Dati.Domanda[0].CodZona.Substring(1, 2));

                                        bool isDomandaLavorabilePerEccezione = GestioneCtrlBypassTipologieNonAbilitate.IsDomandaLavorabilePerEccezione(tipoAppartenenza, codiceSede,
                                            datiDomanda.Dati.Istanza[0].CodGruppo, datiDomanda.Dati.Istanza[0].CodProdotto, datiDomanda.Dati.Istanza[0].CodTipo, datiDomanda.Dati.Istanza[0].SiglaCatLav,
                                            codTipoRichiesta, Utility.IsDomandaINPDAP(datiDomanda.Dati.Istanza[0].CodGestione));

                                        string codSituazione = datiDomanda.Dati.Fase[datiDomanda.Dati.Fase.Count - 1].CodSituazione;
                                        string codFase = datiDomanda.Dati.Fase[datiDomanda.Dati.Fase.Count - 1].CodFase;
                                        GestioneAreaRiepilogo.IsDomandaLavorabile(codSituazione, codFase, IndConvInt, datiDomanda.Dati.Istanza[0].CodGestione, datiDomanda.Dati.Istanza[0].SiglaCatLav,
                                            datiDomanda.Dati.Istanza[0].CodGruppo, datiDomanda.Dati.Istanza[0].CodProdotto, datiDomanda.Dati.Istanza[0].CodTipo, codTipoRichiesta,
                                            isDomandaLavorabilePerEccezione, out errori);
                                    }

                                if (String.IsNullOrEmpty(errori))
                                    elencoDatiDomanda.Add(datiDomanda);
                            }
                            else
                                messaggioVideoStrBuilder.Append(string.Format("{0} - {1}<br/>", domandaCF.NumDomanda, errori));
                        }

                        messaggioVideo = messaggioVideoStrBuilder.ToString();

                        if (!string.IsNullOrEmpty(messaggioVideo))
                            return false;
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore tecnico durante il recupero delle domande per codice fiscale";
                        string parametri = string.Format("Codice fiscale: {0}; Codice relazione soggetto: {1}", codiceFiscale, codRelazioneSoggetto);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        internal static bool InsertAnagraficaFromWebDom(ServiceReferences.WebDom.DatiDomanda datiDomanda, out string errori)
        {
            errori = "";
            using (new MethodExecutionTracer())
            {
                try
                {
                    if (datiDomanda == null || datiDomanda.Dati == null || datiDomanda.Dati.Soggetto == null || datiDomanda.Dati.Soggetto.Count == 0)
                    {
                        errori = "Area dati soggetto WebDom non valorizzata";
                        return false;
                    }

                    foreach (DataSetDomanda.SoggettoRow soggetto in datiDomanda.Dati.Soggetto)
                    {
                        BLCommon.GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
                        NormalizzaAnagraficaWebDomToDB(soggetto, out datiAnagrafici);
                        BLCommon.GestioneAnagrafica.SalvaAnagrafica(datiAnagrafici);
                    }

                }
                catch (Exception Ex)
                {
                    errori = Ex.Message;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    return false;
                }
            }
            return true;
        }

        internal static void NormalizzaAnagraficaWebDomToDB(DataSetDomanda.SoggettoRow soggetto, out BLCommon.GestioneAnagrafica.DatiAnagrafici datiAnagrafici)
        {
            datiAnagrafici = new GestioneAnagrafica.DatiAnagrafici();
            datiAnagrafici.CodiceFiscale = soggetto["CodiceFiscale"] != DBNull.Value && soggetto.CodiceFiscale != null ? soggetto.CodiceFiscale.ToUpperInvariant().Trim() : null;
            datiAnagrafici.Cognome = soggetto["Cognome"] != DBNull.Value && soggetto.Cognome != null ? soggetto.Cognome.ToUpperInvariant().Trim() : null;
            datiAnagrafici.Nome = soggetto["Nome"] != DBNull.Value && soggetto.Nome != null ? soggetto.Nome.ToUpperInvariant().Trim() : null;
            datiAnagrafici.CognomeAcquisito = soggetto["CognomeAcquisito"] != DBNull.Value && soggetto.CognomeAcquisito != null ? soggetto.CognomeAcquisito.ToUpperInvariant().Trim() : null;
            datiAnagrafici.Sesso = soggetto["Sesso"] != DBNull.Value && soggetto.Sesso != null ? Utility.StringToNullableChar(soggetto.Sesso.ToUpperInvariant().Trim()) : null;
            datiAnagrafici.DataNascita = soggetto["DataNascita"] != DBNull.Value && soggetto.DataNascita != null ? Utility.DataFromString(soggetto.DataNascita, Utility.FormatoData.AAAAmmGG) : null;
            datiAnagrafici.CodiceComuneNascita = soggetto["CodComuneNascita"] != DBNull.Value && soggetto.CodComuneNascita != null ? soggetto.CodComuneNascita.ToUpperInvariant().Trim() : null;
            datiAnagrafici.ComuneNascita = soggetto["ComuneNascita"] != DBNull.Value && soggetto.ComuneNascita != null ? soggetto.ComuneNascita.ToUpperInvariant().Trim() : null;
            datiAnagrafici.ProvinciaNascita = soggetto["SiglaProvinciaNascita"] != DBNull.Value && soggetto.SiglaProvinciaNascita != null ? soggetto.SiglaProvinciaNascita.ToUpperInvariant().Trim() : null;
            datiAnagrafici.CodiceComuneResidenza = soggetto["CodComuneResidenza"] != DBNull.Value && soggetto.CodComuneResidenza != null ? soggetto.CodComuneResidenza.ToUpperInvariant().Trim() : null;
            datiAnagrafici.ComuneResidenza = soggetto["ComuneResidenza"] != DBNull.Value && soggetto.ComuneResidenza != null ? soggetto.ComuneResidenza.ToUpperInvariant().Trim() : null;
            datiAnagrafici.ProvinciaResidenza = soggetto["SiglaProvinciaResidenza"] != DBNull.Value && soggetto.SiglaProvinciaResidenza != null ? soggetto.SiglaProvinciaResidenza.ToUpperInvariant().Trim() : null;
            datiAnagrafici.FrazioneResidenza = soggetto["FrazioneResidenza"] != DBNull.Value && soggetto.FrazioneResidenza != null ? soggetto.FrazioneResidenza.ToUpperInvariant().Trim() : null;
            datiAnagrafici.CAP = soggetto["CapResidenza"] != DBNull.Value && soggetto.CapResidenza != null ? soggetto.CapResidenza.ToUpperInvariant().Trim() : null;
            datiAnagrafici.NCivico = soggetto["NumeroCivico"] != DBNull.Value && soggetto.NumeroCivico != null ? soggetto.NumeroCivico.ToUpperInvariant().Trim() : null;
            datiAnagrafici.Indirizzo = soggetto["Indirizzo1"] != DBNull.Value && soggetto.Indirizzo1 != null ? soggetto.Indirizzo1.ToUpperInvariant().Trim() : null;
            datiAnagrafici.Tel = soggetto["NumeroTelefono1"] != DBNull.Value && soggetto.NumeroTelefono1 != null ? soggetto.NumeroTelefono1.ToUpperInvariant().Trim() : null;
            datiAnagrafici.Cell = soggetto["NumeroTelefono2"] != DBNull.Value && soggetto.NumeroTelefono2 != null ? soggetto.NumeroTelefono2.ToUpperInvariant().Trim() : null;
            datiAnagrafici.EMail = soggetto["IndirizzoEmail"] != DBNull.Value && soggetto.IndirizzoEmail != null ? soggetto.IndirizzoEmail.ToUpperInvariant().Trim() : null;
            datiAnagrafici.ResidenzaEstero = soggetto["IndResidenteEstero"] != DBNull.Value && soggetto.IndResidenteEstero != null ? soggetto.IndResidenteEstero == "2" ? true : (bool?)false : null;
            datiAnagrafici.CodiceStatoCivile = soggetto["StatoCivile"] != DBNull.Value && soggetto.StatoCivile != null && soggetto.StatoCivile != "0" && soggetto.StatoCivile != "9" ? Utility.StringToNullableChar(soggetto.StatoCivile.ToUpperInvariant().Trim()) : null;
            if (datiAnagrafici.CodiceStatoCivile.HasValue)
            {
                List<char> elencoStatiCiviliAmmessi = new List<char> { '1', '2', '3', '4', '5', '7', '8', 'C' };
                if (!elencoStatiCiviliAmmessi.Contains(datiAnagrafici.CodiceStatoCivile.Value))
                    datiAnagrafici.CodiceStatoCivile = null;
            }
            datiAnagrafici.DecorrenzaStatoCivile = soggetto["DataDecorrStatoCivile"] != DBNull.Value && soggetto.DataDecorrStatoCivile != null ? Utility.DataFromString(soggetto.DataDecorrStatoCivile, Utility.FormatoData.AAAAmmGG) : null;
            string codCatastale = string.Empty;
            if (string.IsNullOrEmpty(datiAnagrafici.CodiceComuneNascita))
            {
                if (!string.IsNullOrEmpty(datiAnagrafici.ComuneNascita) && !string.IsNullOrEmpty(datiAnagrafici.ProvinciaNascita))
                {

                    GestioneDecodifica.GetCodiceCatastalePerComune_Provincia(datiAnagrafici.ComuneNascita, datiAnagrafici.ProvinciaNascita, out codCatastale);
                    if (!string.IsNullOrEmpty(codCatastale))
                        datiAnagrafici.CodiceComuneNascita = codCatastale;
                }
            }
            if (string.IsNullOrEmpty(datiAnagrafici.CodiceComuneResidenza))
            {
                if (!string.IsNullOrEmpty(datiAnagrafici.ComuneResidenza) && !string.IsNullOrEmpty(datiAnagrafici.ProvinciaResidenza))
                {
                    codCatastale = string.Empty;
                    GestioneDecodifica.GetCodiceCatastalePerComune_Provincia(datiAnagrafici.ComuneResidenza, datiAnagrafici.ProvinciaResidenza, out codCatastale);
                    if (!string.IsNullOrEmpty(codCatastale))
                        datiAnagrafici.CodiceComuneResidenza = codCatastale;
                }
                else if (!string.IsNullOrEmpty(datiAnagrafici.CAP))
                {
                    codCatastale = string.Empty;
                    GestioneDecodifica.GetCodiceCatastalePerCap(datiAnagrafici.CAP, out codCatastale);
                    if (!string.IsNullOrEmpty(codCatastale))
                        datiAnagrafici.CodiceComuneResidenza = codCatastale;
                }
            }
        }

        private static bool GetSedeDestinazioneByResidenza(long numeroDomanda, string codCatastale, string cap, out string sedeDestinazione, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            sedeDestinazione = null;
            WSWebDomSoapClient proxy = null;

            Identity identity = new Identity();
            Guid guid = Guid.NewGuid();
            string stackTrace = null;

            using (new MethodExecutionTracer())
            {
                try
                {
                    string errore;
                    proxy = new WSWebDomSoapClient();
                    GestioneLogSoap.SalvaLogSoap(new AreaInputGetSedeDestinazioneByResidenza { CodCatastale = codCatastale, CAP = cap }, Utility.Servizio.SrvWebDom, Utility.MetodoServizio.GetSedeDestinazioneByResidenza, Utility.SOAPLogDirection.IN, numeroDomanda.ToString(), guid);
                    sedeDestinazione = proxy.GetSedeDestinazioneByResidenza(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "",
                        codCatastale, cap, out errore);
                    if (!String.IsNullOrEmpty(errore))
                    {
                        messaggioVideo = "Errore tecnico durante il recupero delle informazioni della domanda: " + errore;
                        return false;
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore tecnico durante il recupero della sede di destinazione dalla residenza";
                        string parametri = string.Format("Codice catastale: {0}, CAP: {1}", codCatastale, cap);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(new AreaOutputGetSedeDestinazioneByResidenza { SedeDestinazione = sedeDestinazione }, Utility.Servizio.SrvWebDom, Utility.MetodoServizio.GetSedeDestinazioneByResidenza, Utility.SOAPLogDirection.OUT, numeroDomanda.ToString(), guid);
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        internal static bool GetListCodUnitaProcessoBySedeDestinazione(string codiceProvincia, string codiceZona, string codiceCentroOperativoDestinazione, out List<Processo> listCodUnitaProcesso, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            WSWebDomSoapClient proxy = null;
            string stackTrace = null;
            listCodUnitaProcesso = null;
            Processo[] arrayAppCodUnitaProcesso = null;
            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    string errore = string.Empty;

                    string codProvincia = !string.IsNullOrEmpty(codiceProvincia) ? codiceProvincia.PadLeft(3, '0') : "000";
                    string codZona = !string.IsNullOrEmpty(codiceZona) ? codiceZona.PadLeft(3, '0') : "000";
                    string codCentroOperativo = !string.IsNullOrEmpty(codiceCentroOperativoDestinazione) ? codiceCentroOperativoDestinazione.PadLeft(3, '0') : "000";

                    proxy = new WSWebDomSoapClient();
                    arrayAppCodUnitaProcesso = proxy.ListaProcessiPerProvZonaCO(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "",
                        codProvincia, codZona, codCentroOperativo, ref errore);

                    if (!String.IsNullOrEmpty(errore))
                    {
                        messaggioVideo = "Errore tecnico durante il recupero delle informazioni della domanda: " + errore;
                        return false;
                    }
                    else
                    {
                        if (arrayAppCodUnitaProcesso == null || arrayAppCodUnitaProcesso.Count() == 0)
                        {
                            messaggioVideo = string.Format("Non è stato possibile ricavare la lista di  codici unità processo dalla sede di destinazione {0}{1}",
                                   codProvincia,
                                   codCentroOperativo);
                            return false;
                        }
                        listCodUnitaProcesso = arrayAppCodUnitaProcesso.ToList();
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore tecnico durante il recupero la lista di codici unità di processo dalla sede di destinazione";
                        string parametri = string.Format("Codice sede destinazione: {0}; Centro operativo destinazione: {1}", codiceProvincia, codiceCentroOperativoDestinazione);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        //ENG - Metodo LeggiEsitiSanitario
        internal static bool GetEsitiSanitario(string numDomanda, out RispostaEsiti rispostaEsiti, out string errori)
        {
            bool erroreTecnico = false;
            errori = string.Empty;
            AreaAgg areaAgg = new AreaAgg();
            WSWebDomSoapClient proxy = null;
            Guid guid = Guid.NewGuid();
            string stackTrace = null;
            rispostaEsiti = new RispostaEsiti();
            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    if (numDomanda == null || numDomanda.Trim() == String.Empty)
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: Chiave di ricerca non assegnata correttamente. Non è possibile recuperare i dati richiesti";
                        return false;
                    }
                    areaAgg.NumeroDomanda = numDomanda.ToString();
                    GestioneLogSoap.SalvaLogSoap(areaAgg, Utility.Servizio.SrvWebDom, Utility.MetodoServizio.LeggiEsitiSanitario, Utility.SOAPLogDirection.IN, numDomanda.ToString(), guid);
                    proxy = new WSWebDomSoapClient();
                    rispostaEsiti = proxy.LeggiEsitiSanitario(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "",
                    numDomanda);
                    if (rispostaEsiti != null && !String.IsNullOrEmpty(rispostaEsiti.Errore) && !String.IsNullOrEmpty(rispostaEsiti.Errore.Trim()))
                    {
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda: " + rispostaEsiti.Errore;
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
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    errori = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    errori = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    errori = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    errori = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(errori);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(errori) && erroreTecnico)
                    {
                        string messaggio = errori;
                        errori = "Errore tecnico durante il recupero delle informazioni della domanda";
                        long numeroDomanda = 0;
                        long.TryParse(numDomanda, out numeroDomanda);
                        GestioneLogGenerico.SalvaLogGenerico(numeroDomanda, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, stackTrace);
                    }
                    GestioneLogSoap.SalvaLogSoap(rispostaEsiti, Utility.Servizio.SrvWebDom, Utility.MetodoServizio.LeggiEsitiSanitario, Utility.SOAPLogDirection.OUT, numDomanda, guid);
                    Utility.CloseClient(proxy);
                }
            }
            return true;
        }

        private static bool GetCodUnitaProcessoBySedeDestinazione(short? codiceSedeDestinazione, byte? centroOperativoDestinazione, string gestione, out byte? codUnitaProcesso, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            codUnitaProcesso = null;
            WSWebDomSoapClient proxy = null;
            string stackTrace = null;

            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    string errore = string.Empty;

                    string codProvincia = codiceSedeDestinazione.HasValue ? codiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0') : "000";
                    string codZona = codiceSedeDestinazione.HasValue ? codiceSedeDestinazione.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0') : "000";
                    string codCO = centroOperativoDestinazione.HasValue ? centroOperativoDestinazione.ToString().PadLeft(3, '0') : "000";

                    proxy = new WSWebDomSoapClient();
                    Processo[] listaProcesso = proxy.ListaProcessiPerProvZonaCO(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "",
                        codProvincia, codZona, codCO, ref errore);
                    if (!String.IsNullOrEmpty(errore))
                    {
                        messaggioVideo = "Errore tecnico durante il recupero delle informazioni della domanda: " + errore;
                        return false;
                    }
                    else
                    {
                        if (listaProcesso != null && listaProcesso.Length > 0)
                        {
                            Processo processo = listaProcesso.Where(x => !string.IsNullOrEmpty(x.Codice)).OrderBy(x =>
                            {
                                if (x.Descrizione.ToUpperInvariant() == "ASSICURATO PENSIONATO")
                                    return -400;
                                else if (gestione != "019" && x.Descrizione.ToUpperInvariant().Contains("ASSICURATO PENSIONATO GESTIONE PRIVATA"))
                                    return -300;
                                else if (gestione != "019" && x.Descrizione.ToUpperInvariant().Contains("NUCLEO BASE DI SERVIZI STANDARD"))
                                    return -200;
                                else if (gestione == "019" && x.Descrizione.ToUpperInvariant().Contains("ASSICURATO PENSIONATO GESTIONE PUBBLICA"))
                                    return -100;
                                else
                                {
                                    byte codice = 0;
                                    byte.TryParse(x.Codice, out codice);
                                    return codice;
                                }
                            }).FirstOrDefault();

                            if (processo != null)
                            {
                                byte codice = 0;
                                if (byte.TryParse(processo.Codice, out codice))
                                    codUnitaProcesso = codice;
                            }
                            else
                            {
                                messaggioVideo = string.Format("Non è stato possibile ricavare il codice unità processo dalla sede di destinazione {0}{1}",
                                    codiceSedeDestinazione.HasValue ? codiceSedeDestinazione.Value.ToString().PadLeft(4, '0') : "0000",
                                    centroOperativoDestinazione.HasValue ? centroOperativoDestinazione.Value.ToString().PadLeft(2, '0') : "00");
                                return false;
                            }
                        }
                        else
                        {
                            messaggioVideo = string.Format("Non è stato possibile ricavare il codice unità processo dalla sede di destinazione {0}{1}",
                                   codiceSedeDestinazione.HasValue ? codiceSedeDestinazione.Value.ToString().PadLeft(4, '0') : "0000",
                                   centroOperativoDestinazione.HasValue ? centroOperativoDestinazione.Value.ToString().PadLeft(2, '0') : "00");
                            return false;
                        }
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore tecnico durante il recupero dell'unità di processo dalla sede di destinazione";
                        string parametri = string.Format("Codice sede destinazione: {0}; Centro operativo destinazione: {1}", codiceSedeDestinazione, centroOperativoDestinazione);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool GetCodUnitaProcessoBySedeGP1ALZ6(short? codiceSedeGP1ALZ6, byte? centroOperativoGP1ALZ6, string gestione, out byte? codUnitaProcessoGP1ALZ6, out string messaggioVideo)
        {
            bool erroreTecnico = false;
            messaggioVideo = string.Empty;
            codUnitaProcessoGP1ALZ6 = null;
            WSWebDomSoapClient proxy = null;
            string stackTrace = null;

            Identity identity = new Identity();

            using (new MethodExecutionTracer())
            {
                try
                {
                    string errore = string.Empty;

                    string codProvincia = codiceSedeGP1ALZ6.HasValue ? codiceSedeGP1ALZ6.ToString().PadLeft(4, '0').Substring(0, 2).PadLeft(3, '0') : "000";
                    string codZona = codiceSedeGP1ALZ6.HasValue ? codiceSedeGP1ALZ6.ToString().PadLeft(4, '0').Substring(2, 2).PadLeft(3, '0') : "000";
                    string codCO = centroOperativoGP1ALZ6.HasValue ? centroOperativoGP1ALZ6.ToString().PadLeft(3, '0') : "000";

                    proxy = new WSWebDomSoapClient();
                    Processo[] listaProcesso = proxy.ListaProcessiPerProvZonaCO(ref identity, ConfigurationManager.AppSettings["WEBDOM-CODE"] != null ? ConfigurationManager.AppSettings["WEBDOM-CODE"] : "",
                        codProvincia, codZona, codCO, ref errore);
                    if (!String.IsNullOrEmpty(errore))
                    {
                        messaggioVideo = "Errore tecnico durante il recupero delle informazioni della domanda: " + errore;
                        return false;
                    }
                    else
                    {
                        if (listaProcesso != null && listaProcesso.Length > 0)
                        {
                            Processo processo = listaProcesso.Where(x => !string.IsNullOrEmpty(x.Codice)).OrderBy(x =>
                            {
                                if (x.Descrizione.ToUpperInvariant() == "ASSICURATO PENSIONATO")
                                    return -400;
                                else if (gestione != "019" && x.Descrizione.ToUpperInvariant().Contains("ASSICURATO PENSIONATO GESTIONE PRIVATA"))
                                    return -300;
                                else if (gestione != "019" && x.Descrizione.ToUpperInvariant().Contains("NUCLEO BASE DI SERVIZI STANDARD"))
                                    return -200;
                                else if (gestione == "019" && x.Descrizione.ToUpperInvariant().Contains("ASSICURATO PENSIONATO GESTIONE PUBBLICA"))
                                    return -100;
                                else
                                {
                                    byte codice = 0;
                                    byte.TryParse(x.Codice, out codice);
                                    return codice;
                                }
                            }).FirstOrDefault();

                            if (processo != null)
                            {
                                byte codice = 0;
                                if (byte.TryParse(processo.Codice, out codice))
                                    codUnitaProcessoGP1ALZ6 = codice;
                            }
                            else
                            {
                                messaggioVideo = string.Format("Non è stato possibile ricavare il codice unità processo dalla sede del GP1ALZ6 {0}{1}",
                                    codiceSedeGP1ALZ6.HasValue ? codiceSedeGP1ALZ6.Value.ToString().PadLeft(4, '0') : "0000",
                                    centroOperativoGP1ALZ6.HasValue ? centroOperativoGP1ALZ6.Value.ToString().PadLeft(2, '0') : "00");
                                return false;
                            }
                        }
                        else
                        {
                            messaggioVideo = string.Format("Non è stato possibile ricavare il codice unità processo dalla sede del GP1ALZ6 {0}{1}",
                                   codiceSedeGP1ALZ6.HasValue ? codiceSedeGP1ALZ6.Value.ToString().PadLeft(4, '0') : "0000",
                                   centroOperativoGP1ALZ6.HasValue ? centroOperativoGP1ALZ6.Value.ToString().PadLeft(2, '0') : "00");
                            return false;
                        }
                    }
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaApplicationFaultContract> exception)
                {
                    messaggioVideo = Utility.GetMessageFromException(exception);
                    stackTrace = exception.StackTrace;
                    erroreTecnico = true;
                    return false;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaInfrastructureFaultContract>)
                {
                    throw;
                }
                catch (FaultException<INPS.DNA.Services.FaultContract.DnaSecurityFaultContract> Ex)
                {
                    messaggioVideo = string.Format("Si è verificato un errore di sicurezza nel consumo del servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.EndpointNotFoundException Ex)
                {
                    messaggioVideo = string.Format("Puntamento errato al servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (System.ServiceModel.CommunicationException Ex)
                {
                    messaggioVideo = string.Format("Errore di comunicazione con il servizio WebDom | {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.LogException(Ex);
                    erroreTecnico = true;
                    return false;
                }
                catch (Exception Ex)
                {
                    messaggioVideo = string.Format("Errore nel consumo del servizio WebDom: {0}", Utility.GetMessageFromException(Ex));
                    stackTrace = Ex.StackTrace;
                    INPS.DNA.Logging.Logger.WriteError(messaggioVideo);
                    erroreTecnico = true;
                    return false;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(messaggioVideo) && erroreTecnico)
                    {
                        string messaggio = messaggioVideo;
                        messaggioVideo = "Errore tecnico durante il recupero dell'unità di processo dalla sede del GP1ALZ6";
                        string parametri = string.Format("Codice sede destinazione: {0}; Centro operativo destinazione: {1}", codiceSedeGP1ALZ6, centroOperativoGP1ALZ6);
                        GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, parametri, stackTrace);
                    }
                    Utility.CloseClient(proxy);
                }
            }

            return true;
        }

        private static bool IsAttivitaAperta(ServiceReferences.WebDom.DatiDomanda datiDomanda, GestioneWebDom.CodiceAttivita attivita)
        {
            bool isAttivitaAperta = false;
            if (datiDomanda != null && datiDomanda.Dati != null && datiDomanda.Dati.Attivita != null && datiDomanda.Dati.Attivita.Rows != null && datiDomanda.Dati.Attivita.Rows.Count > 0)
            {
                for (int i = 0; i < datiDomanda.Dati.Attivita.Rows.Count; i++)
                {
                    string codAttivita = GetCodiceAttivita(datiDomanda, i);
                    if (int.Parse(codAttivita) == (int)attivita)
                    {
                        string dataFineAttivita = datiDomanda.Dati.Attivita[i].DataFine;
                        if (string.IsNullOrEmpty(dataFineAttivita))
                        {
                            isAttivitaAperta = true;
                            break;
                        }
                    }
                }
            }

            return isAttivitaAperta;
        }

        #endregion private members

        public enum CodiceAttivita
        {
            [Description("00118")]
            Prelievo = 118,
            [Description("00121")]
            InAcquisizione = 121,
            [Description("00110")]
            AttesaCalcolo = 110,
            [Description("00111")]
            CollegamentoConOrganismoEstero,
            [Description("00200")]
            CalcoloEsatto = 200,
            [Description("00113")]
            CalcoloErrato = 113,
            [Description("00107")]
            AvvioIterCI = 107,
            [Description("00201")]
            CalcoloProvvisorio = 201,
            [Description("00212")]
            CalcoloProvvisorioEMENS = 212,
            [Description("00231")]
            CalcoloProvvisorioDMAG = 231,
            [Description("00202")]
            Trasferita = 202,
            [Description("00101")]
            RichiestaDocumenti = 101,
            [Description("00104")]
            ChiarimentiNormativi = 104,
            [Description("00114")]
            DecisioneDirigente = 114,
            [Description("00147")]
            DirezioneTerritorialeLavoro = 147,
            [Description("00102")]
            UfficioSanitario = 102,
            [Description("00123")]
            AccoglimentoSIN = 123,
            [Description("00158")]
            InTrattazioneUnicarpe = 158,
            [Description("00169")]
            InTrattazioneSPI = 169,
            [Description("00168")]
            InAttesaDiCalcoloAutomatico = 168,
            [Description("00130")]
            CorrispondenzaConIlRichiedente = 130
        };

        [Serializable]
        private class AreaInputGetSedeDestinazioneByResidenza
        {
            public string CodCatastale { get; set; }
            public string CAP { get; set; }
        }

        [Serializable]
        private class AreaOutputGetSedeDestinazioneByResidenza
        {
            public string SedeDestinazione { get; set; }
        }

        public class WebDomJson
        {
            public class Rootobject
            {
                public Datiaggiuntivirichiestaesodo DatiAggiuntiviRichiestaEsodo { get; set; }
            }

            public class Datiaggiuntivirichiestaesodo
            {
                public Item CodiceEnte { get; set; }
                public Item RetribuzioneMensile { get; set; }
                public Item SettimaneIncremento { get; set; }
                public Assegnostraordinario AssegnoStraordinario { get; set; }
                public Calcolotfr CalcoloTFR { get; set; }
                public Contribuzionecorrelata ContribuzioneCorrelata { get; set; }
                public Periodoassicurativo PeriodoAssicurativo { get; set; }
                public Pianoesodo PianoEsodo { get; set; }
                public Item RiduzioneAssegno { get; set; }
            }

            public class Assegnostraordinario
            {
                public Item DataFineErogazione { get; set; }
            }

            public class Calcolotfr
            {
                public Item Aliquota { get; set; }
                public Item DataFine { get; set; }
                public Item DataInizio { get; set; }
            }

            public class Contribuzionecorrelata
            {
                public Item DataFineVersamenti { get; set; }
            }

            public class Periodoassicurativo
            {
                public Item DataFine { get; set; }
                public Item DataInizio { get; set; }
            }

            public class Pianoesodo
            {
                public Item AbiBancaFideiussoria { get; set; }
                public Item CabBancaFideiussoria { get; set; }
                public Item AnnoPiano { get; set; }
                public Item NumeroProgressivo { get; set; }
            }

            public class Item
            {
                public string value { get; set; }
                public string description { get; set; }
            }
        }
    }
}
