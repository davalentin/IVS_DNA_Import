using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.Pensioni.Liquidazione.BLCommon.Entity;
using INPS.Pensioni.Liquidazione;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEAziendeVESO33
    {
        public static void GetDecodificaAziendeEAziendeVESO33(string siglaCategoria, out List<Entity.AziendeVESO33> elencoAziendeVESO33)
        {
            elencoAziendeVESO33 = new List<Entity.AziendeVESO33>();

            //lista delle aziende veso 33
            List<GestioneAziendeVESO33.DecAziendeVESO33> elencoAzVESO33BL = new List<GestioneAziendeVESO33.DecAziendeVESO33>();
            GestioneAziendeVESO33.GetDecodificaAziendeVESO33(out elencoAzVESO33BL);

            //lista delle aziende
            List<GestioneDecodificaAzienda.DecAzienda> elAzVESO33 = new List<GestioneDecodificaAzienda.DecAzienda>();
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(siglaCategoria, null, out elAzVESO33);

            //unione delle due liste al nuovo tipo dell'entity
            foreach (GestioneDecodificaAzienda.DecAzienda azienda in elAzVESO33)
            {

                Entity.AziendeVESO33 az = new Entity.AziendeVESO33();
                if (elencoAzVESO33BL != null)
                {
                    GestioneAziendeVESO33.DecAziendeVESO33 aziendaVESO33 = elencoAzVESO33BL.Find(x => x.CodiceAzienda == azienda.Id);
                    if (aziendaVESO33 != null)
                    {
                        az.UltimaDecorrenzaAmmessa = aziendaVESO33.UltimaDecorrenzaAmmessa;
                    }
                }
                az.CodiceAziendaTraduzioneSuGP = azienda.TraduzioneSuGP;
                az.Descrizione = azienda.Descrizione;
                elencoAziendeVESO33.Add(az);

            }
        }

        public static void SalvaAziendeVESO33(Entity.AziendeVESO33 aziendaVESO33ToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziendeVESO33(aziendaVESO33ToSave, out messaggioVideo))
                return;
            else
            {
                GestioneAziendeVESO33.DecAziendeVESO33 aziendaVESO33split = new GestioneAziendeVESO33.DecAziendeVESO33();
                GestioneDecodificaAzienda.DecAzienda aziendasplit = new GestioneDecodificaAzienda.DecAzienda();

                aziendasplit.TraduzioneSuGP = aziendaVESO33ToSave.CodiceAziendaTraduzioneSuGP;
                aziendasplit.Descrizione = aziendaVESO33ToSave.Descrizione;
                aziendaVESO33split.UltimaDecorrenzaAmmessa = aziendaVESO33ToSave.UltimaDecorrenzaAmmessa;
                GestioneAziendeVESO33.SalvaAziendeVESO33(aziendaVESO33split, aziendasplit);
            }
        }

        public static void DeleteAziendeVESO33(Entity.AziendeVESO33 aziendaVESO33ToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAziendeVESO33.DeleteAziendaVESO33(aziendaVESO33ToDelete.CodiceAziendaTraduzioneSuGP);
        }

        private static bool ControlliAziendeVESO33(Entity.AziendeVESO33 aziendaVESO33ToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (aziendaVESO33ToSave == null)
            {
                messaggioVideo = "Nessuna Azienda VESO33 da salvare";
                return false;
            }

            //// controllo codiceAzienda, Descrizione e UltimaDecorrenzaAmmessa obbligatori
            if (string.IsNullOrEmpty(aziendaVESO33ToSave.CodiceAziendaTraduzioneSuGP) || string.IsNullOrEmpty(aziendaVESO33ToSave.Descrizione) || !aziendaVESO33ToSave.UltimaDecorrenzaAmmessa.HasValue)
            {
                messaggioVideo = "Codice Azienda, Descrizione e  Ultima Decorrenza Ammessa sono campi obbligatori";
                return false;
            }

            return true;
        }

    }
}
