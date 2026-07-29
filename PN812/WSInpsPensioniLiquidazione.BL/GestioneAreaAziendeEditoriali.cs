using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEditoriali
    {
        #region anagraficaAccordi

        public static void GetDecodificaAnagraficaAccordi(out List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> elencoAnagraficaAccordi)
        {
            elencoAnagraficaAccordi = null;
            GestioneAnagraficaAccordi.GetDecAnagraficaAccordi(out elencoAnagraficaAccordi);
        }

        public static void SalvaAnagraficaAccordi(GestioneAnagraficaAccordi.DecodAnagraficaAccordi anagraficaAccordi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAnagraficaAccordi(anagraficaAccordi, out messaggioVideo))
                return;
            else
                GestioneAnagraficaAccordi.SalvaAnagraficaAccordi(anagraficaAccordi);
        }

        public static void DeleteAnagraficaAccordi(GestioneAnagraficaAccordi.DecodAnagraficaAccordi anagraficaAccordi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            int result = GestioneAnagraficaAccordi.DeleteAnagraficaAccordi(anagraficaAccordi);

            if (result == -2)
                messaggioVideo = "Record in uso, impossibile eliminare";
        }

        #endregion anagraficaAccordi

        #region anagraficaAziende

        public static void GetDecodificaAnagraficaAziende(out List<GestioneAnagraficaAziende.DecodAnagraficaAziende> elencoAnagraficaAziende)
        {
            elencoAnagraficaAziende = null;
            GestioneAnagraficaAziende.GetDecAnagraficaAziende(out elencoAnagraficaAziende);
        }

        public static void SalvaAnagraficaAziende(GestioneAnagraficaAziende.DecodAnagraficaAziende anagraficaAziende, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAnagraficaAziende(anagraficaAziende, out messaggioVideo))
                return;
            else
                GestioneAnagraficaAziende.SalvaAnagraficaAziende(anagraficaAziende);
        }

        public static void DeleteAnagraficaAziende(GestioneAnagraficaAziende.DecodAnagraficaAziende anagraficaAziende, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAnagraficaAziende.DeleteAnagraficaAziende(anagraficaAziende);
        }

        #endregion anagraficaAziende

        #region private methods
        /// <summary>
        /// logica di controllo dell'inserimento update delle Anagrafica Accordi
        /// </summary>
        /// <param name="anagraficaAccordi"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        private static bool ControlliAnagraficaAccordi(GestioneAnagraficaAccordi.DecodAnagraficaAccordi anagraficaAccordi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (anagraficaAccordi == null)
            {
                messaggioVideo = "Nessun Anagrafica Accordi da salvare";
                return false;
            }

            //// controllo tutti i campi obbligatori
            if (anagraficaAccordi.Abilitata == null || !anagraficaAccordi.Codice.HasValue || (anagraficaAccordi.DataAccordi == null && !(anagraficaAccordi.Codice == 530 || anagraficaAccordi.Codice == 536 || anagraficaAccordi.Codice == 537 || anagraficaAccordi.Codice == 999))
                || string.IsNullOrEmpty(anagraficaAccordi.Decreto) || anagraficaAccordi.DenominazioneAzienda == null || anagraficaAccordi.DomandeLiquidabili == null || anagraficaAccordi.DomandeLiquidate == null)
            {
                messaggioVideo = "Tutti i campi sono obbligatori";
                return false;
            }

            ///controllo record già presente nel db,(se tutti i campi sono uguali tranne id) non deve essere caricato nella lista. overload del metodo equals
            List<GestioneAnagraficaAccordi.DecodAnagraficaAccordi> elencoAnagraficaAccordiDB = null;
            GestioneAreaAziendeEditoriali.GetDecodificaAnagraficaAccordi(out elencoAnagraficaAccordiDB);


            //controllo Codice già esistente
            if (elencoAnagraficaAccordiDB != null && elencoAnagraficaAccordiDB.Exists(x => x.Codice.GetValueOrDefault() == anagraficaAccordi.Codice.GetValueOrDefault() && x.Id != anagraficaAccordi.Id))
            {
                messaggioVideo = "Impossibile inserire il record: Codice già in uso";
                return false;
            }

            //controllo tripletta Codice, DataAccordi e DomandeLiquidabili già esistenti
            if (elencoAnagraficaAccordiDB != null && elencoAnagraficaAccordiDB.Exists(x => x.Codice.GetValueOrDefault() == anagraficaAccordi.Codice.GetValueOrDefault() && x.DataAccordi == anagraficaAccordi.DataAccordi && x.DomandeLiquidabili == anagraficaAccordi.DomandeLiquidabili && x.Id != anagraficaAccordi.Id))
            {
                messaggioVideo = "Impossibile inserire il record: DataAccordi e DomandeLiquidabili già in uso";
                return false;
            }

            //controllo codice azienda inserito dalla gridview banche già presente nella tabella aziende
            List<GestioneAnagraficaAziende.DecodAnagraficaAziende> elencoAnagraficaAziendeDB = null;
            GestioneAnagraficaAziende.GetDecAnagraficaAziende(out elencoAnagraficaAziendeDB);

            if (anagraficaAccordi.DenominazioneAzienda != null && elencoAnagraficaAziendeDB != null && !elencoAnagraficaAziendeDB.Exists(x => x.Id == anagraficaAccordi.DenominazioneAzienda))
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
        private static bool ControlliAnagraficaAziende(GestioneAnagraficaAziende.DecodAnagraficaAziende anagraficaAziende, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (anagraficaAziende == null)
            {
                messaggioVideo = "Nessun Anagrafica Aziende da salvare";
                return false;
            }

            //// controllo tutti i campi obbligatori
            if (string.IsNullOrEmpty(anagraficaAziende.DenominazioneAzienda) || anagraficaAziende.SottogruppoOnere == null)
            {
                messaggioVideo = "Tutti i campi sono obbligatori";
                return false;
            }

            ///controllo record già presente nel db,(se tutti i campi sono uguali tranne id) non deve essere caricato nella lista. overload del metodo equals
            List<GestioneAnagraficaAziende.DecodAnagraficaAziende> elencoAnagraficaAziendeDB = null;
            GestioneAreaAziendeEditoriali.GetDecodificaAnagraficaAziende(out elencoAnagraficaAziendeDB);


            //controllo tripletta DenominazioneAzienda e Oneri già esistenti
            if (anagraficaAziende.Id == 0 && elencoAnagraficaAziendeDB != null && elencoAnagraficaAziendeDB.Exists(x => (x.DenominazioneAzienda != null ? x.DenominazioneAzienda.Trim().ToUpperInvariant() : null) == anagraficaAziende.DenominazioneAzienda.Trim().ToUpperInvariant() && x.SottogruppoOnere == anagraficaAziende.SottogruppoOnere))
            {
                messaggioVideo = "Impossibile inserire il record: DenominazioneAzienda e Oneri già in uso";
                return false;
            }

            return true;
        }

        #endregion private methods
    }
}
