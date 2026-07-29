using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEAziendeESOTRA
    {
        public static void GetDecodificaAziendeEAziendeESOTRA(out List<Entity.AziendeESOTRA> elencoAziendeESOTRA)
        {
            elencoAziendeESOTRA = new List<Entity.AziendeESOTRA>();

            //lista delle aziende esotra
            List<GestioneAziendeESOTRA.DecAziendeESOTRA> elencoAzESOTRABL = new List<GestioneAziendeESOTRA.DecAziendeESOTRA>();
            GestioneAziendeESOTRA.GetDecodificaAziendeESOTRA(out elencoAzESOTRABL);

            //lista delle aziende
            List<GestioneDecodificaAzienda.DecAzienda> elAzESOTRA = new List<GestioneDecodificaAzienda.DecAzienda>();
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("ESOTRA", null, out elAzESOTRA);

            //unione delle due liste al nuovo tipo dell'entity
            foreach (GestioneDecodificaAzienda.DecAzienda azienda in elAzESOTRA)
            {
                Entity.AziendeESOTRA az = new Entity.AziendeESOTRA();
                if (elencoAzESOTRABL != null)
                {
                    GestioneAziendeESOTRA.DecAziendeESOTRA aziendaESOTRA = elencoAzESOTRABL.Find(x => x.CodiceAzienda == azienda.Id);
                    if (aziendaESOTRA != null)
                        az.UltimaDecorrenzaAmmessa = aziendaESOTRA.UltimaDecorrenzaAmmessa;
                }
                az.CodiceAziendaTraduzioneSuGP = azienda.TraduzioneSuGP;
                az.Descrizione = azienda.Descrizione;
                elencoAziendeESOTRA.Add(az);
            }
        }

        public static void SalvaAziendeESOTRA(Entity.AziendeESOTRA aziendaESOTRAToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziendeESOTRA(aziendaESOTRAToSave, out messaggioVideo))
                return;
            else
            {
                GestioneAziendeESOTRA.DecAziendeESOTRA aziendaESOTRAsplit = new GestioneAziendeESOTRA.DecAziendeESOTRA();
                GestioneDecodificaAzienda.DecAzienda aziendasplit = new GestioneDecodificaAzienda.DecAzienda();

                aziendasplit.TraduzioneSuGP = aziendaESOTRAToSave.CodiceAziendaTraduzioneSuGP;
                aziendasplit.Descrizione = aziendaESOTRAToSave.Descrizione;
                aziendaESOTRAsplit.UltimaDecorrenzaAmmessa = aziendaESOTRAToSave.UltimaDecorrenzaAmmessa;
                GestioneAziendeESOTRA.SalvaAziendeESOTRA(aziendaESOTRAsplit, aziendasplit);
            }
        }

        public static void DeleteAziendeESOTRA(Entity.AziendeESOTRA aziendaESOTRAToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAziendeESOTRA.DeleteAziendaESOTRA(aziendaESOTRAToDelete.CodiceAziendaTraduzioneSuGP);
        }

        private static bool ControlliAziendeESOTRA(Entity.AziendeESOTRA aziendaESOTRAToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (aziendaESOTRAToSave == null)
            {
                messaggioVideo = "Nessuna Azienda ESOTRA da salvare";
                return false;
            }

            //// controllo codiceAzienda, Descrizione e UltimaDecorrenzaAmmessa obbligatori
            if (string.IsNullOrEmpty(aziendaESOTRAToSave.CodiceAziendaTraduzioneSuGP) || string.IsNullOrEmpty(aziendaESOTRAToSave.Descrizione) || !aziendaESOTRAToSave.UltimaDecorrenzaAmmessa.HasValue)
            {
                messaggioVideo = "Codice Azienda, Descrizione e Ultima Decorrenza Ammessa sono campi obbligatori";
                return false;
            }

            return true;
        }
    }
}
