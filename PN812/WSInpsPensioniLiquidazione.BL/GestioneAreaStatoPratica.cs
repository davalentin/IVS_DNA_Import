using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.Entity;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaStatoPratica
    {
        public static bool GetStatoPratica(out List<Entity.DomandaDettagliata> elencoDomandeDettagliate,
            string nome, string cognome, string codiceFiscale, string sede, string categoria, string tipo, string fondo, short? statoPensione,
            string numeroDomanda, int? certificato, DateTime dataPresentazioneDomandaMin, DateTime dataPresentazioneDomandaMax,
            DateTime dataElaborazioneMin, DateTime dataElaborazioneMax, string matricola, Utility.TipoAppartenenza tipoAppOperatore, Utility.Ruolo ruolo, GestioneStatoPratica.TipoDomanda tipoDomandaInLavorazione,
            GestioneStatoPratica.TipoDomanda tipoDomandaLavorata, string gruppo, string prodotto, string cassa, int sedeDiAppartenenzaOperatore, out string errori)
        {
            errori = "";
            elencoDomandeDettagliate = null;
            List<GestioneStatoPratica.DatiDomandaDettagliata> elencoDatiDomandaDettagliata = null;
            Int64 numDomanda;

            if (Int64.TryParse(numeroDomanda, out numDomanda))
            {
                // se come criterio di ricerca viene utilizzato il numero di domanda, non serve applicare gli altri criteri:
                // il risultato potrà essere solo uno (o nessuno, se il numero di domanda non esiste)
                GestioneStatoPratica.GetPensioneByNumeroDomanda(numDomanda, matricola, sedeDiAppartenenzaOperatore, out elencoDatiDomandaDettagliata);
            }
            else
            {
                // se come criterio di ricerca NON viene utilizzato il numero di domanda, si possono applicare gli altri criteri
                GestioneStatoPratica.GetPensioniByCriteriMultipliOptimized(nome, cognome, codiceFiscale, sede, categoria, tipo, fondo, statoPensione,
                    certificato, dataPresentazioneDomandaMin, dataPresentazioneDomandaMax, dataElaborazioneMin, dataElaborazioneMax,
                    matricola, tipoAppOperatore, ruolo, tipoDomandaInLavorazione, tipoDomandaLavorata, gruppo, prodotto, cassa, out elencoDatiDomandaDettagliata);
            }

            if (elencoDatiDomandaDettagliata != null)
            {
                elencoDomandeDettagliate = new List<Entity.DomandaDettagliata>();
                foreach (GestioneStatoPratica.DatiDomandaDettagliata ddd in elencoDatiDomandaDettagliata)
                {
                    Entity.DomandaDettagliata domandaDettagliata = new DomandaDettagliata();
                    Utility.ValorizzaOggetti(ddd, domandaDettagliata);
                    elencoDomandeDettagliate.Add(domandaDettagliata);
                }
            }
            return true;
        }

        public static void ControllaInfoPratica(GestionePensione.DatiPensione datiPensione, string matricolaOperatore, short sedeOperatore, ref bool IsCalcoloAbilitato, out string statoPensione, out string matricolaUtenteAcquisizione, out string errori)
        {
            statoPensione = string.Empty;
            errori = string.Empty;
            byte? statoPrecedente = datiPensione != null ? datiPensione.StatoPensione : null;
            byte? stPensioneID = null;
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                bool IsGestioneAttivita = false;
                BLCommon.GestioneStatoPratica.AggiornaInfoPratica(datiPensione, IsCalcoloAbilitato, out IsGestioneAttivita, out stPensioneID, out matricolaUtenteAcquisizione);
                if (stPensioneID.HasValue)
                    GestioneDecodifica.GetStatoPensioneById(stPensioneID.Value, out statoPensione);

                //gestione attività WebDom
                if (ConfigurationManager.AppSettings["BypassAttivitaWebDom"] != null &&
                                ConfigurationManager.AppSettings["BypassAttivitaWebDom"] == "SI")
                {
                    IsGestioneAttivita = false;
                }

                if (IsGestioneAttivita)
                {
                    if (!IsCalcoloAbilitato)
                    //{
                    //    GestioneWebDom.ChiusuraAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.InAcquisizione, out errori);
                    //    if (!string.IsNullOrEmpty(errori))
                    //        IsCalcoloAbilitato = false;
                    //    else
                    //    {
                    //        GestioneWebDom.AperturaAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.AttesaCalcolo, out errori);
                    //        if (!string.IsNullOrEmpty(errori))
                    //            IsCalcoloAbilitato = false;
                    //        else
                    //            transactionScope.Complete();
                    //    }
                    //}
                    //else
                    {
                        GestioneWebDom.CodiceAttivita? codAttivita = null;
                        string dataFineAttivita = string.Empty;
                        GestioneWebDom.GetUltimaAttivita(datiPensione.NDomus, out codAttivita, out dataFineAttivita, out errori);

                        if (!string.IsNullOrEmpty(dataFineAttivita) || !codAttivita.HasValue || codAttivita.Value != GestioneWebDom.CodiceAttivita.InAcquisizione)
                        {
                            GestioneWebDom.ChiusuraUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, out errori);
                            if (!string.IsNullOrEmpty(errori))
                                IsCalcoloAbilitato = true;
                            else
                            {
                                GestioneWebDom.AperturaAttivita(datiPensione, matricolaOperatore, sedeOperatore, GestioneWebDom.CodiceAttivita.InAcquisizione, out errori);
                                if (!string.IsNullOrEmpty(errori))
                                    IsCalcoloAbilitato = true;
                                else
                                    transactionScope.Complete();
                            }
                        }
                        else
                            transactionScope.Complete();
                    }
                    else
                        transactionScope.Complete();
                }
                else
                    transactionScope.Complete();
            }
            //SCRIWO
            if (stPensioneID.HasValue && datiPensione != null)
                datiPensione.StatoPensione = stPensioneID;
            GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, statoPrecedente, matricolaOperatore, sedeOperatore);
        }
    }
}
