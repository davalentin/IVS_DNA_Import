using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEAziendeVOESO
    {
        public static void GetDecodificaAziendeEAziendeVOESO(string tipo, out List<Entity.AziendeVOESO> elencoAziendeVOESO)
        {
            elencoAziendeVOESO = new List<Entity.AziendeVOESO>();

            //lista delle aziende 
            List<GestioneAziendeVOESO.DecAziendeVOESO> elencoAzVOESOBL = new List<GestioneAziendeVOESO.DecAziendeVOESO>();
            GestioneAziendeVOESO.GetDecodificaAziendeVOESO(out elencoAzVOESOBL);

            //lista delle aziende
            List<GestioneDecodificaAzienda.DecAzienda> elAzVOESO = new List<GestioneDecodificaAzienda.DecAzienda>();
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VOESO", tipo, out elAzVOESO);
            
            //unione delle due liste al nuovo tipo dell'entity
            foreach (GestioneDecodificaAzienda.DecAzienda azienda in elAzVOESO)
            {
                Entity.AziendeVOESO az = new Entity.AziendeVOESO();
                if (elencoAzVOESOBL != null)
                {
                    GestioneAziendeVOESO.DecAziendeVOESO aziendaVOESO = elencoAzVOESOBL.Find(x => x.CodiceAzienda == azienda.Id);
                    if (aziendaVOESO != null)
                        az.UltimaDecorrenzaAmmessa = aziendaVOESO.UltimaDecorrenzaAmmessa;
                }
                az.CodiceAziendaTraduzioneSuGP = azienda.TraduzioneSuGP;
                az.Descrizione = azienda.Descrizione;
                az.Tipo = azienda.Tipo;
                elencoAziendeVOESO.Add(az);
            }
        }

        public static void SalvaAziendeVOESO(Entity.AziendeVOESO aziendaVOESOToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziendeVOESO(aziendaVOESOToSave, out messaggioVideo))
                return;
            else
            {
                GestioneAziendeVOESO.DecAziendeVOESO aziendaVOESOsplit = new GestioneAziendeVOESO.DecAziendeVOESO();
                GestioneDecodificaAzienda.DecAzienda aziendasplit = new GestioneDecodificaAzienda.DecAzienda();

                aziendasplit.TraduzioneSuGP = aziendaVOESOToSave.CodiceAziendaTraduzioneSuGP;
                aziendasplit.Descrizione = aziendaVOESOToSave.Descrizione;
                aziendaVOESOsplit.UltimaDecorrenzaAmmessa = aziendaVOESOToSave.UltimaDecorrenzaAmmessa;
                aziendaVOESOsplit.Tipo = aziendaVOESOToSave.Tipo;
                GestioneAziendeVOESO.SalvaAziendeVOESO(aziendaVOESOsplit, aziendasplit);
            }
        }

        public static void DeleteAziendeVOESO(Entity.AziendeVOESO aziendaVOESOToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            GestioneAziendeVOESO.DeleteAziendaVOESO(aziendaVOESOToDelete.CodiceAziendaTraduzioneSuGP, aziendaVOESOToDelete.Tipo);
        }

        private static bool ControlliAziendeVOESO(Entity.AziendeVOESO aziendaVOESOToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (aziendaVOESOToSave == null)
            {
                messaggioVideo = "Nessuna Azienda VOESO da salvare";
                return false;
            }

            //// controllo codiceAzienda, Descrizione e Ultima Decorrenza Ammessa obbligatori
            if (string.IsNullOrEmpty(aziendaVOESOToSave.CodiceAziendaTraduzioneSuGP) ||
                string.IsNullOrEmpty(aziendaVOESOToSave.Descrizione) ||
                !aziendaVOESOToSave.UltimaDecorrenzaAmmessa.HasValue)
            {
                messaggioVideo = "Codice Azienda, Descrizione e Ultima Decorrenza Ammessa sono campi obbligatori";
                return false;
            }

            return true;
        }
    }
}
