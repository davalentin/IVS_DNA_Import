using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Transactions;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaPensione
    {
        public static bool EliminaPensione(long numeroDomanda, GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore,
            Utility.TipoAppartenenza tipoAppRuolo, Utility.Ruolo ruolo, int sedeDiAppartenenzaOperatore, out string errore)
        {
            /*Gestione Log Cancellazione*/
            bool LogCancellazione = false;
            GestioneControlliDinamici.ControlloDinamico controlloDinamicoCancellazione = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("AbilitaLogCancellazione", out controlloDinamicoCancellazione);
            if (controlloDinamicoCancellazione == null || controlloDinamicoCancellazione.ValoreControllo == "SI")
                LogCancellazione = true;
            /**/

            errore = string.Empty;
            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "Pima Fase", "", "");
            if (datiPensione == null)
            {
                errore = "Non è stata individuata nessuna domanda con numero: " + numeroDomanda.ToString();
                return false;
            }

            bool isSprenotazione = false;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            short codCategoria = 0;
            short sede = 0;
            int certificato = 0;
            bool isRiapertura = Utility.IsRiaperturaDomanda(datiPensione.Id);
            byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;

            List<GestioneDecodifica.DecSede> elencoDecSede = null;
            GestioneDecodifica.GetElencoDecSede(out elencoDecSede);

            short codiceSedeLavorazione = Utility.GetCodiceSedeLavorazione(datiPensione, isRiapertura);
            byte? centroOperativoLavorazione = Utility.GetCentroOperativoLavorazione(datiPensione, isRiapertura);

            //31/01/2022: verifico se la sede della domanda è chiusa e si trova nella stessa provincia della sede di appartenenza dell'operatore
            GestioneDecodifica.DecSede decSedeChiusa = null;
            bool isSedeChiusaStessaProvinciaOperatore = false;
            if (elencoDecSede != null && elencoDecSede.Count > 0)
            {
                decSedeChiusa = elencoDecSede.FindAll(x => !String.IsNullOrEmpty(x.CodProvincia) && codiceSedeLavorazione.ToString().PadLeft(4, '0').Substring(0, 2) == x.CodProvincia.PadLeft(3, '0').Substring(1, 2)
                     && !String.IsNullOrEmpty(x.CodZona) && codiceSedeLavorazione.ToString().PadLeft(4, '0').Substring(2, 2) == x.CodZona.PadLeft(3, '0').Substring(1, 2)
                     && !String.IsNullOrEmpty(x.CodCentroOperativo) && centroOperativoLavorazione.HasValue && centroOperativoLavorazione.ToString().PadLeft(2, '0').Substring(0, 2) == x.CodCentroOperativo.PadLeft(3, '0').Substring(1, 2)
                     && x.CodAttivitaSede.GetValueOrDefault() == '0').FirstOrDefault();
                isSedeChiusaStessaProvinciaOperatore = (decSedeChiusa != null && !String.IsNullOrEmpty(decSedeChiusa.CodProvincia)) ? decSedeChiusa.CodProvincia.PadLeft(3, '0').Substring(1, 2) == sedeDiAppartenenzaOperatore.ToString().PadLeft(6, '0').Substring(0, 2) : false;
            }

            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "BypassControlloSedi", "", "");
            //controllo sede operatore - sede domanda
            if (ConfigurationManager.AppSettings["BypassControlloSedi"] == null ||
                    ConfigurationManager.AppSettings["BypassControlloSedi"] != "SI")
            {
                //controllo sede operatore - sede domanda
                if (!GestioneAreaRiepilogo.CheckSedi(codiceSedeLavorazione, (centroOperativoLavorazione.HasValue ? (short)centroOperativoLavorazione.Value : (short)0), sedeOperatore, centroOperativoOperatore) && !isSedeChiusaStessaProvinciaOperatore)
                {
                    errore = "La sede dell'operatore non coincide con la sede della domanda selezionata (" +
                       codiceSedeLavorazione.ToString().PadLeft(4, '0') +
                        (centroOperativoLavorazione.HasValue ? centroOperativoLavorazione.Value.ToString().PadLeft(2, '0') : "00") + ").";
                    return false;
                }
            }

            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "Seconda Fase", "", "");
            //Controllo se la domanda è lavorabile da parte dell'operatore
            if (tipoAppRuolo != tipoAppartenenza)
            {
                errore = "Ruolo Utente non abilitato alla lavorazione della domanda.";
                return false;
            }

            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "BypassControlloMatricola", "", "");
            //controllo matricola operatore
            if (ConfigurationManager.AppSettings["BypassControlloMatricola"] == null ||
                    ConfigurationManager.AppSettings["BypassControlloMatricola"] != "SI")
            {
                bool isGestionePubblica = datiPensione.Gestione == "019";
                bool isAutomatizzata = datiPensione.TipoAutomazione != null;
                if (!GestioneAreaRiepilogo.CheckMatricolaAcquisizione(matricolaOperatore, datiPensione.MatricolaUtenteAcquisizione, ruolo, isAutomatizzata , isGestionePubblica, tipoAppartenenza) && !isSedeChiusaStessaProvinciaOperatore)
                {
                    errore = "Domanda in carico alla matricola " + datiPensione.MatricolaUtenteAcquisizione + ". Non è possibile lavorarla";
                    return false;
                }
            }

            GestioneControlliDinamici.ControlloDinamico controlloDinamico = null;
            GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("BypassEliminazioneRedditi", out controlloDinamico);

            if (tipoAppartenenza == Utility.TipoAppartenenza.FS && (Utility.IsDomandaReversibilita(datiPensione) || Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiapertura)))
            {
                string codCategoriaStr = string.Empty;
                isSprenotazione = true;
                if (Utility.IsDomandaReversibilita(datiPensione))
                {
                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    GestioneDanteCausa.GetDanteCausaByIdPensione(datiPensione.Id, out datiDanteCausa);
                    if (datiDanteCausa != null)
                    {
                        GestioneDecodifica.GetCodCategoriaBySiglaCategoria(datiDanteCausa.SiglaCategoria, out codCategoriaStr);
                        short.TryParse(datiDanteCausa.Sede, out sede);
                        certificato = datiDanteCausa.Certificato.GetValueOrDefault();
                    }
                }
                else
                {
                    codCategoriaStr = datiPensione.GetCodCategoria();
                    sede = datiPensione.CodiceSedeDestinazione.HasValue ? datiPensione.CodiceSedeDestinazione.Value : datiPensione.CodiceSede;
                    certificato = datiPensione.NCertificato.GetValueOrDefault();
                }
                short.TryParse(codCategoriaStr, out codCategoria);
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "BypassAttivitaWebDom", "", "");
                //gestione attività WebDom
                bool isGestioneAttivita = true;
                if (ConfigurationManager.AppSettings["BypassAttivitaWebDom"] != null &&
                        ConfigurationManager.AppSettings["BypassAttivitaWebDom"] == "SI")
                {
                    isGestioneAttivita = false;
                }

                //gestione DRED
                bool isGestioneRedditi = true;
                if (controlloDinamico == null || controlloDinamico.ValoreControllo == "SI")
                    isGestioneRedditi = false;

                if (isGestioneAttivita)
                {
                    if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "ChiusuraUltimaAttivita", "", "");
                    GestioneWebDom.ChiusuraUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, out errore);
                    if (!string.IsNullOrEmpty(errore))
                    {
                        GestionePensione.DatiByPassCancellazione datiByPassCancellazione = null;
                        GestionePensione.GetByPassCancellazione(datiPensione.NDomus, Utility.GetCodiceSedeLavorazione(datiPensione, isRiapertura), Utility.GetCentroOperativoLavorazione(datiPensione, isRiapertura), datiPensione.SiglaCategoria, out datiByPassCancellazione);

                        if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "datiByPassCancellazione", "", "");
                        if (datiByPassCancellazione != null)
                        {
                            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "EliminaByPassCancellazione", "", "");
                            GestionePensione.EliminaByPassCancellazione(datiByPassCancellazione);
                        }
                        else
                            return false;
                    }
                    else
                        if (datiPensione.TipoFelpe == (byte)Utility.TipoFelpe.SIN)
                            GestioneWebDom.AperturaAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.AccoglimentoSIN, out errore);
                }

                if (Utility.IsDomandaENPALS(datiPensione.Gestione) && datiPensione.IsDatiENPALSRecuperati.GetValueOrDefault())
                {
                    TipoRichiesta.SBL? tipoRic = TipoRichiesta.SBL.SBLSAI;
                    if (isRiapertura)
                        tipoRic = TipoRichiesta.SBL.SBLSAY;
                    else if (Utility.IsRicostituzione_Supplemento(datiPensione))
                        tipoRic = TipoRichiesta.SBL.SBLSAS;
                    else if ((Utility.IsRicostituzione_MotiviContributivi(datiPensione)) || (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) == Utility.TipoDomanda.Ricostituzione))
                        tipoRic = TipoRichiesta.SBL.SBLSAR;

                    if (!GestioneSAI.SbloccoSAI(datiPensione.NDomus, tipoRic, out errore))
                        return false;
                }

                if (isGestioneRedditi)
                {
                    if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "ElimninaRedditiSrvRedditiByDatiPensione", "", "");
                    if (!GestioneRedditi.ElimninaRedditiSrvRedditiByDatiPensione(datiPensione, out errore))
                        return false;
                }

                if (isSprenotazione)
                {
                    if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "EseguiSprenotazione", "", "");
                    if (!GestioneLiquidazioneFs.EseguiSprenotazione(datiPensione.NDomus.ToString(), sede, codCategoria, certificato,
                        sedeOperatore, centroOperativoOperatore, datiPensione.Gruppo, datiPensione.Prodotto, isRiapertura, out errore))
                        return false;
                }

                if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "EliminaLogSoap", "", "");
                GestioneLogSoap.EliminaLogSoap(datiPensione.Id);
                if (!LogCancellazione) GestioneLogGenerico.EliminaLogGenerico(datiPensione.NDomus);
                string ParametriPensione = string.Format("SiglaCategoria: {0}, Tipo: {1}, Gruppo: {2}, Prodotto: {3}", datiPensione.SiglaCategoria.Trim(), datiPensione.Tipo, datiPensione.Gruppo, datiPensione.Prodotto);
                string Parametri = "Id: " + datiPensione.Id + " NDomus: " + datiPensione.NDomus + " LogCancellazione: " + LogCancellazione.ToString();
                if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "EliminaPensione", Parametri, "");
                GestionePensione.EliminaPensione(datiPensione.Id, datiPensione.NDomus, LogCancellazione);
                if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "transactionScope.Complete", "", "");

                transactionScope.Complete();
            }
            //SCRIWO
            datiPensione.StatoPensione = (int)Utility.StatoPensione.DaAcquisire;
            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "AggiornaStatoLavorazione", "", "");
            GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, matricolaOperatore, sedeOperatore);

            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "EliminaSbloccoDomanda", "", "");
            //eliminazione atomica da sblocco domanda
            GestioneSbloccoDomanda.DatiSbloccoDomanda datiSbloccoDomanda = new GestioneSbloccoDomanda.DatiSbloccoDomanda();
            datiSbloccoDomanda.NDomus = datiPensione.NDomus;
            GestioneSbloccoDomanda.EliminaSbloccoDomanda(datiSbloccoDomanda);
            if (LogCancellazione) GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, "GestioneAreaPensione.EliminaPensione", Utility.TipoLogGenerico.Informativo, "Fine", "", "");
            return true;
        }
    }
}
