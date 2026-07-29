using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.Entity
{
    public class AltreFunzioni
    {
        public AltreFunzioni()
        {
            IsGestioneLiquidazione = false;
            IsTipologieNonAbilitate = false;
            IsSbloccoDomanda = false;
            IsRiassegnazioneDomanda = false;
            IsMonitoraggio = false;
            IsAvvisi = false;
            IsMessaggiHermes = false;
            IsAggiornamenti = false;
            IsSbloccoCancellazione = false;
            IsBypassControlli = false;
            IsCambioDataSistema = false;
            IsCambioDataINDCOM = false;
            IsGestioneFaq = false;
            IsCambioStatoDomanda = false;
            IsPulisciDomanda = false;
            IsBypassTipologieNonAbilitate = false;
            IsFunzionalitaAggiornamentoPostCalcolo = false;
            IsGestioneTrasformazioni = false;
            IsGestioneAziendeVESO92 = false;
            IsGestioneAziendeVESO33 = false;
            IsGestioneAziendeCredito = false;
            IsGestioneAziendeEditoriali = false;
            IsGestioneAziendeEditorialiLetteraB = false;
            IsGestioneAziendeEditoriali0171 = false;
            IsGestioneAziendeEditoriali0179 = false;
            IsGestioneAziendeVESO29 = false;
            IsGestioneAziendeVOESO = false;
            IsGestioneAziendeESOTEL = false;
            IsGestioneAziendeESOAMB = false;
            IsGestioneAziendeESPA = false;
            IsGestioneProvvisoriePerCoefficienti = false;
            IsGestioneAbilitazioneChiavi = false;
            IsGestioneAziendeESOPMI = false;
        }
        public bool IsGestioneLiquidazione { get; set; }
        public bool IsTipologieNonAbilitate { get; set; }
        public bool IsSbloccoDomanda { get; set; }
        public bool IsRiassegnazioneDomanda { get; set; }
        public bool IsMonitoraggio { get; set; }
        public bool IsAvvisi { get; set; }
        public bool IsMessaggiHermes { get; set; }
        public bool IsAggiornamenti { get; set; }
        public bool IsSbloccoCancellazione { get; set; }
        public bool IsBypassControlli { get; set; }
        public bool IsCambioDataSistema { get; set; }
        public bool IsCambioDataINDCOM { get; set; }
        public bool IsGestioneFaq { get; set; }
        public bool IsCambioStatoDomanda { get; set; }
        public bool IsPulisciDomanda { get; set; }
        public bool IsBypassTipologieNonAbilitate { get; set; }
        public bool IsFunzionalitaAggiornamentoPostCalcolo { get; set; }
        public bool IsGestioneTrasformazioni { get; set; }
        public bool IsGestioneAziendeVESO92 { get; set; }
        public bool IsGestioneAziendeVESO33 { get; set; }
        public bool IsGestioneAziendeCredito { get; set; }
        public bool IsGestioneAziendeEditoriali { get; set; }
        public bool IsGestioneAziendeEditorialiLetteraB { get; set; }
        public bool IsGestioneAziendeEditoriali0171 { get; set; }
        public bool IsGestioneAziendeEditoriali0179 { get; set; }
        public bool IsGestioneAziendeVESO29 { get; set; }
        public bool IsGestioneAziendeVOESO { get; set; }
        public bool IsGestioneAziendeESOTEL { get; set; }
        public bool IsGestioneProvvisoriePerCoefficienti { get; set; }
        public bool IsGestioneAbilitazioneChiavi { get; set; }
        public bool IsGestioneAziendeESOAMB { get; set; }
        public bool IsGestioneAziendeESPA { get; set; }
        public bool IsGestioneAziendeESOPMI { get; set; }
    }
}
