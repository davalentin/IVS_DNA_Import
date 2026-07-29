using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public static class GestioneAreaCambioStatoDomanda
    {
        public static void RicercaDomanda(ref DatiCambiaStatoDomanda datiCambioDatiDomanda, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiPensione == null)
            {
                messaggioVideo = "Numero domanda non presente nel database";
                return;
            }

            Utility.StatoPensione? stato;
            //recupero la descrizione dello stato pensione
            stato = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value);
            datiCambioDatiDomanda.NumeroDomanda = datiPensione.NDomus;
            datiCambioDatiDomanda.StatoPensione = Utility.GetDescription(stato);
            datiCambioDatiDomanda.NCertificato = datiPensione.NCertificato.ToString();
            datiCambioDatiDomanda.DataElaborazioneWebdom = datiPensione.DataElaborazione;

            if (!ControlRicercaDomanda(datiCambioDatiDomanda, datiPensione, out messaggioVideo))
                return;
        }

        public static void AggiornaDomanda(string sNewState, string nuovoCertificato, DateTime? nuovaDataElaborazione,GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (datiPensione == null)
            {
                messaggioVideo = "Impossibile recuperare i dati pensione";
                return;
            }
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            Utility.StatoPensione? newStato = Utility.GetStatoPensioneByDescrizione(sNewState);
            Utility.StatoPensione? oldState = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value);
            string sOldState = Utility.GetDescription(oldState);

            if (!ControlAggiornaStato(tipoAppartenenza, sOldState, sNewState, out messaggioVideo))
                return;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //Aggiorna stato pensione
                datiPensione.StatoPensione = (byte)newStato.Value;
                datiPensione.NCertificato = String.IsNullOrEmpty(nuovoCertificato) ? datiPensione.NCertificato : Utility.StringToNullableInt(nuovoCertificato);
                datiPensione.DataElaborazione = (nuovaDataElaborazione == null) ? datiPensione.DataElaborazione : nuovaDataElaborazione;
                
                GestionePensione.SalvaPensione(datiPensione);

                // Se passo allo stato Calcolata No Webdom, allora se la domanda è di prepensionamento editoria, bisogna aggiornare il contatore delle domande liquidate
                if (newStato.Value == Utility.StatoPensione.CalcolataNoWebDom)
                {
                    bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);
                    if (!Utility.IsRicostituzioneOrRiapertura(datiPensione, isRiaperturaDomanda))
                    {
                        if (Utility.IsPrepensionamentoEditoriaArt1c154L205_2017(datiPensione))
                        {
                            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
                            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);
                            if (datiIstruttoria.CodiceAziendaEditoriaPerTipo0171.HasValue)
                                GestioneAnagraficaAccordiPerTipo0171.UpdateCountLiquidate_AnagraficaAccordi(datiIstruttoria.CodiceAziendaEditoriaPerTipo0171);
                        }
                    }
                }

                transactionScope.Complete();
            }
        }

        private static bool ControlRicercaDomanda(DatiCambiaStatoDomanda datiCambiaStatoDomanda, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (datiCambiaStatoDomanda.NumeroDomanda == 0)
            {
                messaggioVideo = "Il Numero di Domanda non è valorizzato";
                return false;
            }

            if (datiCambiaStatoDomanda.NumeroDomanda.ToString().StartsWith("0") || datiCambiaStatoDomanda.NumeroDomanda.ToString().Length != 13)
            {
                messaggioVideo = "Il Numero di Domanda non può avere come prima cifra 0 e deve essere lungo 13";
                return false;
            }

            Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoApp.HasValue && datiCambiaStatoDomanda.TipoAppOperatore.HasValue && datiCambiaStatoDomanda.TipoAppOperatore.Value != tipoApp)
            {
                messaggioVideo = "Il tipo appartenenza dell'operatore non coincide con il tipo appartenenza della domanda ricercata";
                return false;
            }


            if (datiCambiaStatoDomanda.Ruolo.HasValue &&
                datiCambiaStatoDomanda.Ruolo.Value != Utility.Ruolo.AMMINISTRATORE)
            {
                messaggioVideo = "La il cambio di stato della domanda può essere effettuata solamente da un Amministratore ";
                return false;
            }

            int sede = int.Parse(Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0'));
            if (datiCambiaStatoDomanda.Sede != sede)
            {
                messaggioVideo = "La sede dell'operatore non coincide con la sede della domanda selezionata (" + sede.ToString().PadLeft(6, '0') + ")";
                datiCambiaStatoDomanda.SedeDiversa = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
                return false;
            }
            return true;
        }

        private static bool ControlAggiornaStato(Utility.TipoAppartenenza? tipoAppartenza, string statoPartenza, string statoArrivo, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            // ?? -> DA_CALCOLARE  
            if ((statoArrivo == Utility.GetDescription(Utility.StatoPensione.DaCalcolare) && statoPartenza != Utility.GetDescription(Utility.StatoPensione.InAcquisizione)))
            {
                messaggioVideo = "Solo una domanda in stato '" + Utility.GetDescription(Utility.StatoPensione.InAcquisizione) + "' può essere portata in '" + Utility.GetDescription(Utility.StatoPensione.DaCalcolare) + "'";
                return false;
            }
            // ?? -> CALCOLATA_NO_WEBDOM
            if (statoArrivo == Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom) && 
                statoPartenza != Utility.GetDescription(Utility.StatoPensione.Calcolata) &&
                statoPartenza != Utility.GetDescription(Utility.StatoPensione.ScartoDaCalcolo) &&
                statoPartenza != Utility.GetDescription(Utility.StatoPensione.CalcoloNoIndeb) &&
                statoPartenza != Utility.GetDescription(Utility.StatoPensione.CalcoloNoIndebWait)
                )
            // 29-01-2020: Rimossa modifica ‘Consentito cambio di stato da 3-‘DA CALCOLARE’ a CALCOLO NO WEBDOM D (email del 20 gennaio2020 da Francesco Mele con oggetto: mancata chiusura WEBDOM) 
            //statoPartenza != Utility.GetDescription(Utility.StatoPensione.ScartoDaCalcolo) && !(tipoAppartenza == Utility.TipoAppartenenza.AGO && statoPartenza == Utility.GetDescription(Utility.StatoPensione.DaCalcolare)))
            {
                messaggioVideo = "Una domanda nello stato '" + statoPartenza + "' non può essere portata in '" + Utility.GetDescription(Utility.StatoPensione.CalcolataNoWebDom) + "'";
                return false;
            }
            return true;
        }
        #region nested class
        public class DatiCambiaStatoDomanda
        {
            #region Public Properties

            #region Input/Output Parameters
            public long NumeroDomanda { get; set; }
            public string StatoPensione { get; set; }

            public string NCertificato { get; set; }

            public DateTime? DataElaborazioneWebdom { get; set; }
             
            #endregion Input/Output Parameters

            #region Input Parameters
            public string NuovaMatricola { get; set; }
            public Utility.TipoAppartenenza? TipoAppOperatore { get; set; }
            public Utility.Ruolo? Ruolo { get; set; }
            public int? Sede { get; set; }
            #endregion Input Parameters

            #region Output Parameters
            public string SedeDiversa { get; set; }
            #endregion Output Parameters
            #endregion Public Properties

        }
        #endregion nested class


    }
}
