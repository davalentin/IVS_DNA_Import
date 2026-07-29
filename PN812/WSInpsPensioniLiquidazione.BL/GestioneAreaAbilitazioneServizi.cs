using System;
using System.Collections.Generic;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAbilitazioneServizi
    {
        public static void GetAreaServizi(out Dictionary<string, bool> elencoServizi)
        {
            elencoServizi = new Dictionary<string, bool>();

            bool isPolarizzazioneENPALSAttiva = GestioneControlliDinamici.IsPolarizzazioneENPALSAttiva();
            elencoServizi.Add(GestioneControlliDinamici.Keys.PolarizzazioneENPALSAttiva, isPolarizzazioneENPALSAttiva);

            bool isPolarizzazioneSuperstitiENPALSAttiva = GestioneControlliDinamici.IsPolarizzazioneSuperstitiENPALSAttiva();
            elencoServizi.Add(GestioneControlliDinamici.Keys.PolarizzazioneSuperstitiENPALSAttiva, isPolarizzazioneSuperstitiENPALSAttiva);
        }

        public static void SetAbilitazionePolarizzazioneENPALS(bool isPolarizzazioneENPALSAttiva, bool isPolarizzazioneSuperstitiENPALSAttiva, out string messaggioVideo)
        {
            messaggioVideo = null;
            List<GestioneControlliDinamici.ControlloDinamico> listaControlliDinamici = new List<GestioneControlliDinamici.ControlloDinamico>();

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoPolarizzazione = new GestioneControlliDinamici.ControlloDinamico();
            controlloDinamicoPolarizzazione.NomeControllo = GestioneControlliDinamici.Keys.PolarizzazioneENPALSAttiva;
            if (isPolarizzazioneENPALSAttiva == true)
                controlloDinamicoPolarizzazione.ValoreControllo = "SI";
            else
                controlloDinamicoPolarizzazione.ValoreControllo = "NO";
            listaControlliDinamici.Add(controlloDinamicoPolarizzazione);

            GestioneControlliDinamici.ControlloDinamico controlloDinamicoPolarizzazioneSuperstiti = new GestioneControlliDinamici.ControlloDinamico();
            controlloDinamicoPolarizzazioneSuperstiti.NomeControllo = GestioneControlliDinamici.Keys.PolarizzazioneSuperstitiENPALSAttiva;
            if (isPolarizzazioneSuperstitiENPALSAttiva == true)
                controlloDinamicoPolarizzazioneSuperstiti.ValoreControllo = "SI";
            else
                controlloDinamicoPolarizzazioneSuperstiti.ValoreControllo = "NO";
            listaControlliDinamici.Add(controlloDinamicoPolarizzazioneSuperstiti);

            try
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var controllodinamico in listaControlliDinamici)
                        GestioneControlliDinamici.SalvaControlloDinamico(controllodinamico);
                    transactionScope.Complete();
                }
            }
            catch (Exception Ex)
            {
                messaggioVideo = "Errore durante il salvataggio delle abilitazioni per polarizzazione ENPALS";
                INPS.DNA.Logging.Logger.LogException(Ex);
            }
        }
    }
}
