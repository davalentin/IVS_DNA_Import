using System.Collections.Generic;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAltreFunzioni
    {
        public static void GetAbilitazioniByMatricola(string matricola, out AltreFunzioni altreFunzioni)
        {
            altreFunzioni = new AltreFunzioni();
            List<GestioneCtrlAbilitazioneUtentePerUtility.AbilitazioniUtente> listaAbilitazioni = null;
            GestioneCtrlAbilitazioneUtentePerUtility.GetListaAbilitazioniByMatricola(matricola, out listaAbilitazioni);
            if (listaAbilitazioni != null && listaAbilitazioni.Count > 0)
            {
                foreach (GestioneCtrlAbilitazioneUtentePerUtility.AbilitazioniUtente abilitazione in listaAbilitazioni)
                {
                    switch (abilitazione.MenuUtility)
                    {
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneLiquidazione:
                            altreFunzioni.IsGestioneLiquidazione = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.TipologieNonAbilitate:
                            altreFunzioni.IsTipologieNonAbilitate = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.SbloccoDomanda:
                            altreFunzioni.IsSbloccoDomanda = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.RiassegnazioneDomanda:
                            altreFunzioni.IsRiassegnazioneDomanda = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.Monitoraggio:
                            altreFunzioni.IsMonitoraggio = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.Avvisi:
                            altreFunzioni.IsAvvisi = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.MessaggiHermes:
                            altreFunzioni.IsMessaggiHermes = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.Aggiornamenti:
                            altreFunzioni.IsAggiornamenti = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.SbloccoCancellazione:
                            altreFunzioni.IsSbloccoCancellazione = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.BypassControlli:
                            altreFunzioni.IsBypassControlli = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.CambioDataSistema:
                            altreFunzioni.IsCambioDataSistema = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.CambioDataINDCOM:
                            altreFunzioni.IsCambioDataINDCOM = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneFaq:
                            altreFunzioni.IsGestioneFaq = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.CambioStatoDomanda:
                            altreFunzioni.IsCambioStatoDomanda = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.PulisciDomanda:
                            altreFunzioni.IsPulisciDomanda = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.BypassTipologieNonAbilitate:
                            altreFunzioni.IsBypassTipologieNonAbilitate = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.FunzionalitaAggiornamentoPostCalcolo:
                            altreFunzioni.IsFunzionalitaAggiornamentoPostCalcolo = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneTrasformazioni:
                            altreFunzioni.IsGestioneTrasformazioni = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeVESO92:
                            altreFunzioni.IsGestioneAziendeVESO92 = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeVESO33:
                            altreFunzioni.IsGestioneAziendeVESO33 = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeCredito:
                            altreFunzioni.IsGestioneAziendeCredito = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeEditoriali:
                            altreFunzioni.IsGestioneAziendeEditoriali = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeEditorialiLetteraB:
                            altreFunzioni.IsGestioneAziendeEditorialiLetteraB = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeEditoriali0171:
                            altreFunzioni.IsGestioneAziendeEditoriali0171 = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeEditoriali0179:
                            altreFunzioni.IsGestioneAziendeEditoriali0179 = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeVESO29:
                            altreFunzioni.IsGestioneAziendeVESO29 = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeVOESO:
                            altreFunzioni.IsGestioneAziendeVOESO = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneProvvisoriePerCoefficienti:
                            altreFunzioni.IsGestioneProvvisoriePerCoefficienti = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAbilitazioneChiavi:
                            altreFunzioni.IsGestioneAbilitazioneChiavi = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeESOTEL:
                            altreFunzioni.IsGestioneAziendeESOTEL = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeESOAMB:
                            altreFunzioni.IsGestioneAziendeESOAMB = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeESPA:
                            altreFunzioni.IsGestioneAziendeESPA = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.GestioneAziendeESOPMI:
                            altreFunzioni.IsGestioneAziendeESOPMI = true;
                            break;
                        case GestioneCtrlAbilitazioneUtentePerUtility.MenuUtility.NessunaSelezione:
                        default:
                            break;
                    }
                }
            }
        }
    }
}
