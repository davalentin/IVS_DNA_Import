using System.Collections.Generic;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAziendeEAziendeCredito
    {
        public static void GetDecodificaAziendeEAziendeCredito(string siglaCategoria, out List<Entity.AziendeCredito> elencoAziendeCredito)
        {
            elencoAziendeCredito = new List<Entity.AziendeCredito>();

            //lista delle aziende 
            List<GestioneAziendeCredito.DecAziendeCredito> elencoAzCreditoBL = new List<GestioneAziendeCredito.DecAziendeCredito>();
            GestioneAziendeCredito.GetDecodificaAziendeCredito(out elencoAzCreditoBL);

            //lista delle aziende
            List<GestioneDecodificaAzienda.DecAzienda> elAzCredito = new List<GestioneDecodificaAzienda.DecAzienda>();
            GestioneDecodificaAzienda.GetElencoAziendaBySiglaCategoria(siglaCategoria, null, out elAzCredito);

            //unione delle due liste al nuovo tipo dell'entity
            foreach (GestioneDecodificaAzienda.DecAzienda azienda in elAzCredito)
            {

                Entity.AziendeCredito az = new Entity.AziendeCredito();
                if (elencoAzCreditoBL != null)
                {
                    GestioneAziendeCredito.DecAziendeCredito aziendaCredito = elencoAzCreditoBL.Find(x => x.CodiceAzienda == azienda.Id);
                    if (aziendaCredito != null)
                    {
                        az.UltimaDecorrenzaAmmessa = aziendaCredito.UltimaDecorrenzaAmmessa;
                    }
                }
                az.CodiceAziendaTraduzioneSuGP = azienda.TraduzioneSuGP;
                az.Descrizione = azienda.Descrizione;
                az.SiglaCatPensione = azienda.SiglaCategoria;
                elencoAziendeCredito.Add(az);

            }
        }

        public static void SalvaAziendeCredito(Entity.AziendeCredito aziendaCreditoToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlliAziendeCredito(aziendaCreditoToSave, out messaggioVideo))
                return;
            else
            {
                GestioneAziendeCredito.DecAziendeCredito aziendaCreditosplit = new GestioneAziendeCredito.DecAziendeCredito();
                GestioneDecodificaAzienda.DecAzienda aziendasplit = new GestioneDecodificaAzienda.DecAzienda();

                aziendasplit.TraduzioneSuGP = aziendaCreditoToSave.CodiceAziendaTraduzioneSuGP;
                aziendasplit.Descrizione = aziendaCreditoToSave.Descrizione;
                aziendaCreditosplit.UltimaDecorrenzaAmmessa = aziendaCreditoToSave.UltimaDecorrenzaAmmessa;
                aziendaCreditosplit.SiglaCatPensione = aziendaCreditoToSave.SiglaCatPensione;
                GestioneAziendeCredito.SalvaAziendeCredito(aziendaCreditosplit, aziendasplit);
            }
        }

        public static void DeleteAziendeCredito(Entity.AziendeCredito aziendaCreditoToDelete, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            GestioneAziendeCredito.DeleteAziendaCredito(aziendaCreditoToDelete.CodiceAziendaTraduzioneSuGP);
        }

        private static bool ControlliAziendeCredito(Entity.AziendeCredito aziendaCreditoToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //// controllo inserimento oggetto vuoto
            if (aziendaCreditoToSave == null)
            {
                messaggioVideo = "Nessuna Azienda Credito da salvare";
                return false;
            }

            //// controllo codiceAzienda, Descrizione e Sigla Categoria Pensione obbligatori
            if (string.IsNullOrEmpty(aziendaCreditoToSave.CodiceAziendaTraduzioneSuGP) || 
                string.IsNullOrEmpty(aziendaCreditoToSave.Descrizione) || 
                string.IsNullOrEmpty(aziendaCreditoToSave.SiglaCatPensione))
            {
                messaggioVideo = "Codice Azienda, Descrizione e Sigla Categoria Pensione sono campi obbligatori";
                return false;
            }

            return true;
        }

    }
}
