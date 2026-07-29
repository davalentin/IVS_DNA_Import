using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEAziendeESOAMB
    {
        public static void GetDecodificaAziendeEAziendeESOAMB(out List<Entity.AziendeESOAMB> elencoAziendeESOAMB)
        {
            elencoAziendeESOAMB = new List<Entity.AziendeESOAMB>();

            //lista delle aziende ESOAMB
            List<GestioneAziendeESOAMB.DecAziendeESOAMB> elencoAzESOAMBBL = new List<GestioneAziendeESOAMB.DecAziendeESOAMB>();
            GestioneAziendeESOAMB.GetDecodificaAziendeESOAMB(out elencoAzESOAMBBL);

            //lista delle aziende
            List<GestioneDecodificaAzienda.DecAzienda> elAzESOAMB = new List<GestioneDecodificaAzienda.DecAzienda>();
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("ESOAMB", null, out elAzESOAMB);

            //unione delle due liste al nuovo tipo dell'entity
            foreach (GestioneDecodificaAzienda.DecAzienda azienda in elAzESOAMB)
            {
                Entity.AziendeESOAMB az = new Entity.AziendeESOAMB();
                if (elencoAzESOAMBBL != null)
                {
                    GestioneAziendeESOAMB.DecAziendeESOAMB aziendaESOAMB = elencoAzESOAMBBL.Find(x => x.CodiceAzienda == azienda.Id);
                    if (aziendaESOAMB != null)
                        az.UltimaDecorrenzaAmmessa = aziendaESOAMB.UltimaDecorrenzaAmmessa;
                }
                az.CodiceAziendaTraduzioneSuGP = azienda.TraduzioneSuGP;
                az.Descrizione = azienda.Descrizione;
                elencoAziendeESOAMB.Add(az);
            }
        }

        public static void SalvaAziendeESOAMB(Entity.AziendeESOAMB aziendaESOAMBToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziendeESOAMB(aziendaESOAMBToSave, out messaggioVideo))
                return;
            else
            {
                GestioneAziendeESOAMB.DecAziendeESOAMB aziendaESOAMBsplit = new GestioneAziendeESOAMB.DecAziendeESOAMB();
                GestioneDecodificaAzienda.DecAzienda aziendasplit = new GestioneDecodificaAzienda.DecAzienda();

                aziendasplit.TraduzioneSuGP = aziendaESOAMBToSave.CodiceAziendaTraduzioneSuGP;
                aziendasplit.Descrizione = aziendaESOAMBToSave.Descrizione;
                aziendaESOAMBsplit.UltimaDecorrenzaAmmessa = aziendaESOAMBToSave.UltimaDecorrenzaAmmessa;
                GestioneAziendeESOAMB.SalvaAziendeESOAMB(aziendaESOAMBsplit, aziendasplit);
            }
        }

        public static void DeleteAziendeESOAMB(Entity.AziendeESOAMB aziendaESOAMBToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAziendeESOAMB.DeleteAziendaESOAMB(aziendaESOAMBToDelete.CodiceAziendaTraduzioneSuGP);
        }

        public static void SalvaAziendeScadAssegnoGGmmAAAA(GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aziendaGGmmAAAAtoSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            ControlliAziendeGGmmAAAA(out  messaggioVideo);

            GestioneAziendeScadenzaAssegnoGGmmAAAA.SalvaAziendeScadenzaAssegnoGGmmAAAA(aziendaGGmmAAAAtoSave, "ESOAMB");
        }

        public static void DeleteAziendeScadAssegnoGGmmAAAA(GestioneAziendeScadenzaAssegnoGGmmAAAA.DecAziendeScadenzaAssegnoGGmmAAAA aziendaGGmmAAAAtoDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneAziendeScadenzaAssegnoGGmmAAAA.DeleteAziendeScadenzaAssegnoGGmmAAAA(aziendaGGmmAAAAtoDelete, "ESOAMB");
        }

        private static bool ControlliAziendeESOAMB(Entity.AziendeESOAMB aziendaESOAMBToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (aziendaESOAMBToSave == null)
            {
                messaggioVideo = "Nessuna Azienda ESOAMB da salvare";
                return false;
            }

            //// controllo codiceAzienda, Descrizione e UltimaDecorrenzaAmmessa obbligatori
            if (string.IsNullOrEmpty(aziendaESOAMBToSave.CodiceAziendaTraduzioneSuGP) || string.IsNullOrEmpty(aziendaESOAMBToSave.Descrizione) || !aziendaESOAMBToSave.UltimaDecorrenzaAmmessa.HasValue)
            {
                messaggioVideo = "Codice Azienda, Descrizione e Ultima Decorrenza Ammessa sono campi obbligatori";
                return false;
            }

            return true;
        }

        private static bool ControlliAziendeGGmmAAAA(out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            return true;
        }
    }
}
