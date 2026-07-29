using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEAziendeVESO29
    {
        public static void GetDecodificaAziendeEAziendeVESO29(out List<Entity.AziendeVESO29> elencoAziendeVESO29)
        {
            elencoAziendeVESO29 = new List<Entity.AziendeVESO29>();

            //lista delle aziende veso 29
            List<GestioneAziendeVESO29.DecAziendeVESO29> elencoAzVESO29BL = new List<GestioneAziendeVESO29.DecAziendeVESO29>();
            GestioneAziendeVESO29.GetDecodificaAziendeVESO29(out elencoAzVESO29BL);

            //lista delle aziende
            List<GestioneDecodificaAzienda.DecAzienda> elAzVESO29 = new List<GestioneDecodificaAzienda.DecAzienda>();
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria("VESO29", null, out elAzVESO29);

            //unione delle due liste al nuovo tipo dell'entity
            foreach (GestioneDecodificaAzienda.DecAzienda azienda in elAzVESO29)
            {
                Entity.AziendeVESO29 az = new Entity.AziendeVESO29();
                if (elencoAzVESO29BL != null)
                {
                    GestioneAziendeVESO29.DecAziendeVESO29 aziendaVESO29 = elencoAzVESO29BL.Find(x => x.CodiceAzienda == azienda.Id);
                    if (aziendaVESO29 != null)
                        az.UltimaDecorrenzaAmmessa = aziendaVESO29.UltimaDecorrenzaAmmessa;
                }
                az.CodiceAziendaTraduzioneSuGP = azienda.TraduzioneSuGP;
                az.Descrizione = azienda.Descrizione;
                elencoAziendeVESO29.Add(az);
            }
        }

        public static void SalvaAziendeVESO29(Entity.AziendeVESO29 aziendaVESO29ToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziendeVESO29(aziendaVESO29ToSave, out messaggioVideo))
                return;
            else
            {
                GestioneAziendeVESO29.DecAziendeVESO29 aziendaVESO29split = new GestioneAziendeVESO29.DecAziendeVESO29();
                GestioneDecodificaAzienda.DecAzienda aziendasplit = new GestioneDecodificaAzienda.DecAzienda();

                aziendasplit.TraduzioneSuGP = aziendaVESO29ToSave.CodiceAziendaTraduzioneSuGP;
                aziendasplit.Descrizione = aziendaVESO29ToSave.Descrizione;
                aziendaVESO29split.UltimaDecorrenzaAmmessa = aziendaVESO29ToSave.UltimaDecorrenzaAmmessa;
                GestioneAziendeVESO29.SalvaAziendeVESO29(aziendaVESO29split, aziendasplit);
            }
        }

        public static void DeleteAziendeVESO29(Entity.AziendeVESO29 aziendaVESO29ToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAziendeVESO29.DeleteAziendaVESO29(aziendaVESO29ToDelete.CodiceAziendaTraduzioneSuGP);
        }

        private static bool ControlliAziendeVESO29(Entity.AziendeVESO29 aziendaVESO29ToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (aziendaVESO29ToSave == null)
            {
                messaggioVideo = "Nessuna Azienda VESO29 da salvare";
                return false;
            }

            //// controllo codiceAzienda, Descrizione e UltimaDecorrenzaAmmessa obbligatori
            if (string.IsNullOrEmpty(aziendaVESO29ToSave.CodiceAziendaTraduzioneSuGP) || string.IsNullOrEmpty(aziendaVESO29ToSave.Descrizione) || !aziendaVESO29ToSave.UltimaDecorrenzaAmmessa.HasValue)
            {
                messaggioVideo = "Codice Azienda, Descrizione e Ultima Decorrenza Ammessa sono campi obbligatori";
                return false;
            }

            return true;
        }
    }
}
