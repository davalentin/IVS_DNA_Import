using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEditorialiPerTipo0179
    {
        #region anagraficaAccordi

        public static void GetDecodificaAnagraficaAccordi(out List<Entity.AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordi)
        {
            elencoAnagraficaAccordi = null;
            List<GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179> elencoAnagraficaAccordiDB = null;
            GestioneAnagraficaAccordiPerTipo0179.GetDecAnagraficaAccordi(out elencoAnagraficaAccordiDB);

            if (elencoAnagraficaAccordiDB != null && elencoAnagraficaAccordiDB.Count > 0)
            {
                elencoAnagraficaAccordi = new List<Entity.AnagraficaAccordoPerTipo0179>();
                foreach (var accordoDB in elencoAnagraficaAccordiDB)
                    elencoAnagraficaAccordi.Add(MapAccordoFromBLToView(accordoDB));
            }
        }

        public static void SalvaAnagraficaAccordi(Entity.AnagraficaAccordoPerTipo0179 anagraficaAccordi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAnagraficaAccordi(anagraficaAccordi, out messaggioVideo))
                return;
            else
                GestioneAnagraficaAccordiPerTipo0179.SalvaAnagraficaAccordi(MapAccordoFromViewToBL(anagraficaAccordi));
        }

        public static void DeleteAnagraficaAccordi(Entity.AnagraficaAccordoPerTipo0179 anagraficaAccordi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int result = GestioneAnagraficaAccordiPerTipo0179.DeleteAnagraficaAccordi(MapAccordoFromViewToBL(anagraficaAccordi));

            if (result == -2)
                messaggioVideo = "Record in uso, impossibile eliminare";
        }

        #endregion anagraficaAccordi

        #region anagraficaAziende

        public static void GetDecodificaAnagraficaAziende(out List<Entity.AnagraficaAziendaPerTipo0179> elencoAnagraficaAziende)
        {
            elencoAnagraficaAziende = null;
            List<GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179> elencoAnagraficaAziendeDB = null;
            GestioneAnagraficaAziendePerTipo0179.GetDecAnagraficaAziende(out elencoAnagraficaAziendeDB);

            if (elencoAnagraficaAziendeDB != null && elencoAnagraficaAziendeDB.Count > 0)
            {
                elencoAnagraficaAziende = new List<Entity.AnagraficaAziendaPerTipo0179>();
                foreach (var aziendaDB in elencoAnagraficaAziendeDB)
                    elencoAnagraficaAziende.Add(MapAziendaFromBLToView(aziendaDB));
            }
        }

        public static void SalvaAnagraficaAziende(Entity.AnagraficaAziendaPerTipo0179 anagraficaAziende, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAnagraficaAziende(anagraficaAziende, out messaggioVideo))
                return;
            else
                GestioneAnagraficaAziendePerTipo0179.SalvaAnagraficaAziende(MapAziendaFromViewToBL(anagraficaAziende));
        }

        public static void DeleteAnagraficaAziende(Entity.AnagraficaAziendaPerTipo0179 anagraficaAziende, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAnagraficaAziendePerTipo0179.DeleteAnagraficaAziende(MapAziendaFromViewToBL(anagraficaAziende));
        }

        #endregion anagraficaAziende

        #region private methods

        private static GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179 MapAccordoFromViewToBL(Entity.AnagraficaAccordoPerTipo0179 anagraficaAccordoView)
        {
            GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179 anagraficaAccordoBL = new GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179();
            Utility.ValorizzaOggetti(anagraficaAccordoView, anagraficaAccordoBL);
            return anagraficaAccordoBL;
        }

        private static Entity.AnagraficaAccordoPerTipo0179 MapAccordoFromBLToView(GestioneAnagraficaAccordiPerTipo0179.DecodAnagraficaAccordiPerTipo0179 anagraficaAccordoBL)
        {
            Entity.AnagraficaAccordoPerTipo0179 anagraficaAccordoView = new Entity.AnagraficaAccordoPerTipo0179();
            Utility.ValorizzaOggetti(anagraficaAccordoBL, anagraficaAccordoView);
            return anagraficaAccordoView;
        }

        private static GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179 MapAziendaFromViewToBL(Entity.AnagraficaAziendaPerTipo0179 anagraficaAziendaView)
        {
            GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179 anagraficaAziendaBL = new GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179();
            Utility.ValorizzaOggetti(anagraficaAziendaView, anagraficaAziendaBL);
            return anagraficaAziendaBL;
        }

        private static Entity.AnagraficaAziendaPerTipo0179 MapAziendaFromBLToView(GestioneAnagraficaAziendePerTipo0179.DecodAnagraficaAziendePerTipo0179 anagraficaAziendaBL)
        {
            Entity.AnagraficaAziendaPerTipo0179 anagraficaAziendaView = new Entity.AnagraficaAziendaPerTipo0179();
            Utility.ValorizzaOggetti(anagraficaAziendaBL, anagraficaAziendaView);
            return anagraficaAziendaView;
        }

        /// <summary>
        /// logica di controllo dell'inserimento update delle Anagrafica Accordi
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        private static bool ControlliAnagraficaAccordi(Entity.AnagraficaAccordoPerTipo0179 anagraficaAccordi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (anagraficaAccordi == null)
            {
                messaggioVideo = "Nessun Anagrafica Accordi da salvare";
                return false;
            }

            //// controllo tutti i campi obbligatori
            if (anagraficaAccordi.Abilitata == null || !anagraficaAccordi.Codice.HasValue || anagraficaAccordi.DataAccordi == null
                || anagraficaAccordi.DenominazioneAzienda == null || anagraficaAccordi.DomandeLiquidabili == null || anagraficaAccordi.DomandeLiquidate == null)
            {
                messaggioVideo = "Tutti i campi sono obbligatori";
                return false;
            }

            ///controllo record già presente nel db,(se tutti i campi sono uguali tranne id) non deve essere caricato nella lista. overload del metodo equals
            List<Entity.AnagraficaAccordoPerTipo0179> elencoAnagraficaAccordiDB = null;
            GetDecodificaAnagraficaAccordi(out elencoAnagraficaAccordiDB);

            //controllo Codice già esistente
            if (elencoAnagraficaAccordiDB != null && elencoAnagraficaAccordiDB.Exists(x => x.Codice.GetValueOrDefault() == anagraficaAccordi.Codice.GetValueOrDefault() && x.Id != anagraficaAccordi.Id))
            {
                messaggioVideo = "Impossibile inserire il record: Codice già presente.";
                return false;
            }

            //controllo tripletta Codice, DataAccordi e DomandeLiquidabili già esistenti
            if (elencoAnagraficaAccordiDB != null && elencoAnagraficaAccordiDB.Exists(x => x.Codice.GetValueOrDefault() == anagraficaAccordi.Codice.GetValueOrDefault() && 
                x.DataAccordi == anagraficaAccordi.DataAccordi && x.DomandeLiquidabili == anagraficaAccordi.DomandeLiquidabili && x.Id != anagraficaAccordi.Id))
            {
                messaggioVideo = "Impossibile inserire il record: DataAccordi e DomandeLiquidabili già in uso";
                return false;
            }

            //controllo codice azienda inserito dalla gridview accordi già presente nella tabella aziende
            List<Entity.AnagraficaAziendaPerTipo0179> elencoAnagraficaAziendeDB = null;
            GetDecodificaAnagraficaAziende(out elencoAnagraficaAziendeDB);

            if (anagraficaAccordi.DenominazioneAzienda.HasValue && (elencoAnagraficaAziendeDB == null || !elencoAnagraficaAziendeDB.Exists(x => x.Id == anagraficaAccordi.DenominazioneAzienda)))
            {
                messaggioVideo = "Codice Azienda non esistente: inserire la nuova azienda con relativo Codice Azienda";
                return false;
            }

            return true;
        }

        /// <summary>
        /// logica di controllo dell'inserimento update delle Anagrafica Aziende
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        private static bool ControlliAnagraficaAziende(Entity.AnagraficaAziendaPerTipo0179 anagraficaAziende, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (anagraficaAziende == null)
            {
                messaggioVideo = "Nessun Anagrafica Aziende da salvare";
                return false;
            }

            //// controllo tutti i campi obbligatori
            if (string.IsNullOrEmpty(anagraficaAziende.DenominazioneAzienda) || string.IsNullOrEmpty(anagraficaAziende.SottogruppoPrimoOnere))
            {
                messaggioVideo = "E' obbligatorio indicare la Denominazione Azienda e il Sottogruppo primo onere.";
                return false;
            }

            ///controllo record già presente nel db,(se tutti i campi sono uguali tranne id) non deve essere caricato nella lista. overload del metodo equals
            List<Entity.AnagraficaAziendaPerTipo0179> elencoAnagraficaAziendeDB = null;
            GetDecodificaAnagraficaAziende(out elencoAnagraficaAziendeDB);

            //controllo DenominazioneAzienda già esistente
            if (anagraficaAziende.Id == 0 && elencoAnagraficaAziendeDB != null && elencoAnagraficaAziendeDB.Exists(x => (x.DenominazioneAzienda != null ? x.DenominazioneAzienda.Trim().ToUpperInvariant() : null) == anagraficaAziende.DenominazioneAzienda.Trim().ToUpperInvariant()))
            {
                messaggioVideo = "Impossibile inserire il record: DenominazioneAzienda già in uso";
                return false;
            }

            // controllo che i due sottogruppi non siano uguali
            if (anagraficaAziende.SottogruppoPrimoOnere == anagraficaAziende.SottogruppoSecondoOnere)
            {
                messaggioVideo = "I due sottogruppi non possono essere uguali.";
                return false;
            }

            // controllo che il sottogruppo onere inserito sia esistente
            List<GestioneDecodifica.GruppoOneri> elencoGruppoOneri = null;
            GestioneDecodifica.GetGruppoOneri(out elencoGruppoOneri);

            List<GestioneDecodifica.SottoGruppoOneri> elencoSottoGruppoOneri = null;
            GestioneDecodifica.GetSottoGruppoOneri(out elencoSottoGruppoOneri);

            if (elencoGruppoOneri != null && elencoGruppoOneri.Count > 0 && elencoSottoGruppoOneri != null && elencoSottoGruppoOneri.Count > 0)
            {
                GestioneDecodifica.GruppoOneri onere9000 = elencoGruppoOneri.FirstOrDefault(x => x.Code == "0900");
                List<GestioneDecodifica.SottoGruppoOneri> elencoSottoGruppoOneriAmmessi = null;
                if (onere9000 != null)
                {
                    elencoSottoGruppoOneriAmmessi = elencoSottoGruppoOneri.FindAll(x => x.IdOnere == onere9000.Id);
                    if (!elencoSottoGruppoOneriAmmessi.Exists(x => x.Code == anagraficaAziende.SottogruppoPrimoOnere))
                    {
                        messaggioVideo = string.Format("Il Sottogruppo {0} non è ammesso.", anagraficaAziende.SottogruppoPrimoOnere);
                        return false;
                    }

                    if (!string.IsNullOrEmpty(anagraficaAziende.SottogruppoSecondoOnere) && !elencoSottoGruppoOneriAmmessi.Exists(x => x.Code == anagraficaAziende.SottogruppoSecondoOnere))
                    {
                        messaggioVideo = string.Format("Il Sottogruppo {0} non è ammesso.", anagraficaAziende.SottogruppoSecondoOnere);
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion private methods
    }
}
