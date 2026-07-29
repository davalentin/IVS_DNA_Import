using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaBancaFideiussione
    {
        public static void GetDecodificaBancaFideiussione(out List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiussione)
        {
            elencoBancheFideiussione = null;
            GestioneBancheFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiussione);
        }

        public static void SalvaBancheFideiussione(GestioneBancheFideiussione.DecBancaFideiussione bancaFidToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliBancheFideiussione(bancaFidToSave, out messaggioVideo))
                return;
            else
                GestioneBancheFideiussione.SalvaBancaFideiussione(bancaFidToSave);
        }

        public static void DeleteBancheFideiussione(GestioneBancheFideiussione.DecBancaFideiussione bancaFidToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneBancheFideiussione.DeleteBancaFideiussione(bancaFidToDelete);
        }

        /// <summary>
        /// inserimento aziende
        /// metodo che richiama il BL common 
        /// </summary>
        /// <param name="aziendaToSave"></param>
        /// <param name="messaggioVideo"></param>
        public static void SalvaAziende(GestioneDecodificaAzienda.DecAzienda aziendaToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziende(out messaggioVideo))
                return;
            else
                GestioneDecodificaAzienda.InsertDecodificaAzienda(aziendaToSave);
        }

        public static void SalvaAziendeScadAssegnoGGmmAAAA(GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aziendaGGmmAAAAtoSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            ControlliAziendeGGmmAAAA(out  messaggioVideo);

            GestioneAziendeScadenzaAssegnoGGmmAAAA.SalvaAziendeScadenzaAssegnoGGmmAAAA(aziendaGGmmAAAAtoSave, "VESO92");
        }

        public static void DeleteAziendeScadAssegnoGGmmAAAA(GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aziendaGGmmAAAAtoDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneAziendeScadenzaAssegnoGGmmAAAA.DeleteAziendeScadenzaAssegnoGGmmAAAA(aziendaGGmmAAAAtoDelete, "VESO92");
        }

        #region private methods
        /// <summary>
        /// logica di controllo dell'inserimento update delle banche fideiussione
        /// </summary>
        /// <param name="bancaFideiussione"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        private static bool ControlliBancheFideiussione(GestioneBancheFideiussione.DecBancaFideiussione bancaFideiussione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (bancaFideiussione == null)
            {
                messaggioVideo = "Nessun contratto di Fideiussione da salvare";
                return false;
            }

            //// controllo codice azienda e matricola obbligatori
            if (string.IsNullOrEmpty(bancaFideiussione.CodiceAzienda) || string.IsNullOrEmpty(bancaFideiussione.Matricola))
            {
                messaggioVideo = "Codice Azienda e Matricola campi obbligatori";
                return false;
            }

            ///controllo record già presente nel db,(se tutti i campi sono uguali tranne id) non deve essere caricato nella lista. overload del metodo equals
            List<GestioneBancheFideiussione.DecBancaFideiussione> elencoBancheFideiussioneDB = null;
            GestioneAreaBancaFideiussione.GetDecodificaBancaFideiussione(out elencoBancheFideiussioneDB);

            ///se si inserisce uno tra i campi non obbligatori (codice azienda e matricola obbligatori), devono essere inseriti tutti
            ///cioè o sono tutti valorizzati o nessuno è valorizzato
            ///logica ( || || ||) && (|| || ||) 

            if ((!string.IsNullOrEmpty(bancaFideiussione.BancaFideiussione) ||
                (bancaFideiussione.Progressivo.HasValue) ||
                (bancaFideiussione.Anno.HasValue) ||
                (bancaFideiussione.InizioEsodo.HasValue) ||
                (bancaFideiussione.FineEsodo.HasValue) ||
                (bancaFideiussione.ABI.HasValue) ||
                (bancaFideiussione.CAB.HasValue))
                &&
                (string.IsNullOrEmpty(bancaFideiussione.BancaFideiussione) ||
                !(bancaFideiussione.Progressivo.HasValue) ||
                !(bancaFideiussione.Anno.HasValue) ||
                !(bancaFideiussione.InizioEsodo.HasValue) ||
                !(bancaFideiussione.FineEsodo.HasValue) ||
                !(bancaFideiussione.ABI.HasValue) ||
                !(bancaFideiussione.CAB.HasValue)))
            {
                messaggioVideo = "Per inserire un contratto di fideiussione con una Banca, compilare tutti i campi";
                return false;
            }

            //controllo anno compreso tra data inizio e data fine
            if ((bancaFideiussione.Anno.HasValue) &&
                (bancaFideiussione.InizioEsodo.HasValue) &&
                (bancaFideiussione.FineEsodo.HasValue))
            {
                if ((bancaFideiussione.InizioEsodo.Value.Year) != (bancaFideiussione.FineEsodo.Value.Year))
                {
                    messaggioVideo = "Inizio Esodo e Fine Esodo devono appartenere allo stesso anno";
                    return false;
                }
                if ((bancaFideiussione.InizioEsodo.Value.Year) != (bancaFideiussione.Anno.Value))
                {
                    messaggioVideo = "L'anno inserito deve coincidere con l'anno delle date di inizio e fine esodo";
                    return false;
                }
                if (Utility.DataStrettamenteSuccessivaA(bancaFideiussione.InizioEsodo.Value, bancaFideiussione.FineEsodo.Value))
                {
                    messaggioVideo = "La data di Fine Esodo deve essere successiva alla data di Inizio Esodo";
                    return false;
                }
            }

            //controllo terna codice Azienda, anno, progressivo già esistenti
            if (bancaFideiussione.Id == 0 && elencoBancheFideiussioneDB != null && elencoBancheFideiussioneDB.Exists(x => x.CodiceAzienda == bancaFideiussione.CodiceAzienda && x.Anno == bancaFideiussione.Anno && x.Progressivo == bancaFideiussione.Progressivo))
            {
                messaggioVideo = "Impossibile inserire il record: Codice Azienda, Anno e Progressivo già in uso";
                return false;
            }

            //controllo codice azienda inserito dalla gridview banche già presente nella tabella aziende
            List<GestioneDecodificaAzienda.DecAzienda> elencoAziendeDB = null;
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO92", null, out elencoAziendeDB);

            if (!string.IsNullOrEmpty(bancaFideiussione.CodiceAzienda) && elencoAziendeDB != null && !elencoAziendeDB.Exists(x => x.TraduzioneSuGP == bancaFideiussione.CodiceAzienda))
            {
                messaggioVideo = "Codice Azienda non esistente: inserire la nuova azienda con relativo Codice Azienda";
                return false;
            }

            return true;
        }

        private static bool ControlliAziende(out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            return true;
        }

        private static bool ControlliAziendeGGmmAAAA(out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            return true;
        }

        #endregion private methods
    }
}
