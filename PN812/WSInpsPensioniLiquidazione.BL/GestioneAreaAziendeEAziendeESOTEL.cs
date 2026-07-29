using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEAziendeESOTEL
    {
        public static void GetDecodificaAziendeEAziendeESOTEL(out List<Entity.AziendeESOTEL> elencoAziendeESOTEL)
        {
            elencoAziendeESOTEL = new List<Entity.AziendeESOTEL>();

            //lista delle aziende ESOTEL
            List<GestioneAziendeESOTEL.DecAziendeESOTEL> elencoAzESOTELBL = new List<GestioneAziendeESOTEL.DecAziendeESOTEL>();
            GestioneAziendeESOTEL.GetDecodificaAziendeESOTEL(out elencoAzESOTELBL);

            //lista delle aziende
            List<GestioneDecodificaAzienda.DecAzienda> elAzESOTEL = new List<GestioneDecodificaAzienda.DecAzienda>();
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("ESOTEL", null, out elAzESOTEL);

            //unione delle due liste al nuovo tipo dell'entity
            foreach (GestioneDecodificaAzienda.DecAzienda azienda in elAzESOTEL)
            {
                Entity.AziendeESOTEL az = new Entity.AziendeESOTEL();
                if (elencoAzESOTELBL != null)
                {
                    GestioneAziendeESOTEL.DecAziendeESOTEL aziendaESOTEL = elencoAzESOTELBL.Find(x => x.CodiceAzienda == azienda.Id);
                    if (aziendaESOTEL != null)
                        az.UltimaDecorrenzaAmmessa = aziendaESOTEL.UltimaDecorrenzaAmmessa;
                }
                az.CodiceAziendaTraduzioneSuGP = azienda.TraduzioneSuGP;
                az.Descrizione = azienda.Descrizione;
                elencoAziendeESOTEL.Add(az);
            }
        }

        public static void SalvaAziendeESOTEL(Entity.AziendeESOTEL aziendaESOTELToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziendeESOTEL(aziendaESOTELToSave, out messaggioVideo))
                return;
            else
            {
                GestioneAziendeESOTEL.DecAziendeESOTEL aziendaESOTELsplit = new GestioneAziendeESOTEL.DecAziendeESOTEL();
                GestioneDecodificaAzienda.DecAzienda aziendasplit = new GestioneDecodificaAzienda.DecAzienda();

                aziendasplit.TraduzioneSuGP = aziendaESOTELToSave.CodiceAziendaTraduzioneSuGP;
                aziendasplit.Descrizione = aziendaESOTELToSave.Descrizione;
                aziendaESOTELsplit.UltimaDecorrenzaAmmessa = aziendaESOTELToSave.UltimaDecorrenzaAmmessa;
                GestioneAziendeESOTEL.SalvaAziendeESOTEL(aziendaESOTELsplit, aziendasplit);
            }
        }

        public static void DeleteAziendeESOTEL(Entity.AziendeESOTEL aziendaESOTELToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAziendeESOTEL.DeleteAziendaESOTEL(aziendaESOTELToDelete.CodiceAziendaTraduzioneSuGP);
        }

        private static bool ControlliAziendeESOTEL(Entity.AziendeESOTEL aziendaESOTELToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (aziendaESOTELToSave == null)
            {
                messaggioVideo = "Nessuna Azienda ESOTEL da salvare";
                return false;
            }

            //// controllo codiceAzienda, Descrizione e UltimaDecorrenzaAmmessa obbligatori
            if (string.IsNullOrEmpty(aziendaESOTELToSave.CodiceAziendaTraduzioneSuGP) || string.IsNullOrEmpty(aziendaESOTELToSave.Descrizione) || !aziendaESOTELToSave.UltimaDecorrenzaAmmessa.HasValue)
            {
                messaggioVideo = "Codice Azienda, Descrizione e Ultima Decorrenza Ammessa sono campi obbligatori";
                return false;
            }

            return true;
        }
    }
}
