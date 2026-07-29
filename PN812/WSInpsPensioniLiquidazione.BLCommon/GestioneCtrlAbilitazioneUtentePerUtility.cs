using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlAbilitazioneUtentePerUtility
    {
        public class AbilitazioniUtente
        {
            public string Matricola { get; set; }
            public string Utente { get; set; }
            public string Nome { get; set; }
            public string Cognome { get; set; }
            public MenuUtility MenuUtility { get; set; }
            public string Cod_Utente { get; set; }
            public string Cod_Appl { get; set; }
        }

        public enum MenuUtility
        {
            NessunaSelezione = 0,
            GestioneLiquidazione,
            TipologieNonAbilitate,
            SbloccoDomanda,
            RiassegnazioneDomanda,
            Monitoraggio,
            Avvisi,
            MessaggiHermes,
            Aggiornamenti,
            SbloccoCancellazione,
            BypassControlli,
            CambioDataSistema,
            GestioneFaq,
            CambioStatoDomanda,
            PulisciDomanda,
            BypassTipologieNonAbilitate,
            FunzionalitaAggiornamentoPostCalcolo,
            GestioneTrasformazioni,
            GestioneAziendeVESO92,
            GestioneAziendeVESO33,
            GestioneAziendeCredito,
            GestioneAziendeEditoriali,
            GestioneAziendeEditoriali0171,
            GestioneAziendeVESO29,
            GestioneAziendeVOESO,
            GestioneProvvisoriePerCoefficienti,
            GestioneAbilitazioneChiavi,
            GestioneAziendeESOTEL,
            GestioneAziendeESOAMB,
            GestioneAziendeEditoriali0179,
            GestioneAziendeESPA,
            GestioneAziendeESOPMI,
            CambioDataINDCOM,
            GestioneAziendeEditorialiLetteraB
            //AGGIUNGERE IN CODA! SEMPRE!
        }

        public static void GetListaAbilitazioniByMatricola(string matricola, out List<AbilitazioniUtente> listaAbilitazioni)
        {
            listaAbilitazioni = null;
            List<CtrlAbilitazioneUtentePerUtility> listaAbilitazioniDB = null;
            DAGestioneCtrlAbilitazioneUtentePerUtility.GetListaAbilitazioniByMatricola(matricola, out listaAbilitazioniDB);
            if (listaAbilitazioniDB != null && listaAbilitazioniDB.Count > 0)
            {
                listaAbilitazioni = new List<AbilitazioniUtente>();
                foreach (CtrlAbilitazioneUtentePerUtility abilitazioneDB in listaAbilitazioniDB)
                {
                    AbilitazioniUtente abilitazione = new AbilitazioniUtente();
                    Utility.ValorizzaOggetti(abilitazioneDB, abilitazione);
                    abilitazione.MenuUtility = GetEnumById(abilitazioneDB.Id_MenuUtility);
                    listaAbilitazioni.Add(abilitazione);
                }
            }
        }

        public static MenuUtility GetEnumById(byte? id)
        {
            MenuUtility menuUtility = MenuUtility.NessunaSelezione;
            if (id != null)
            {
                foreach (MenuUtility value in Enum.GetValues(typeof(MenuUtility)))
                {
                    if (value.GetHashCode() == (int)id.GetValueOrDefault())
                    {
                        menuUtility = value;
                        break;
                    }
                }
            }
            return menuUtility;
        }
    }
}
