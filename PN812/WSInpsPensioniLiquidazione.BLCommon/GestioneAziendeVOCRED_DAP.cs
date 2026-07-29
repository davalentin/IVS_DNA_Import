using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAziendeVOCRED_DAP
    {
        public static void GetDecodificaAziendeVOCRED_DAP(out List<DecAziendeVOCRED_DAP> elencoAziendeVOCRED_DAP)
        {
            elencoAziendeVOCRED_DAP = null;
            List<CtrlAziendeVOCRED_DAP> elencoDBAziendeVOCRED_DAP = null;
            DAGestioneCtrlAziendeVOCRED_DAP.GetDecodificaAziendeVOCRED_DAP(out elencoDBAziendeVOCRED_DAP);
            if (elencoDBAziendeVOCRED_DAP != null && elencoDBAziendeVOCRED_DAP.Count > 0)
            {
                elencoAziendeVOCRED_DAP = new List<DecAziendeVOCRED_DAP>();
                foreach (CtrlAziendeVOCRED_DAP decodificaAziendaVOCRED_DAP in elencoDBAziendeVOCRED_DAP)
                {
                    DecAziendeVOCRED_DAP AziendaVOCRED_DAP = new DecAziendeVOCRED_DAP();
                    Utility.ValorizzaOggetti(decodificaAziendaVOCRED_DAP, AziendaVOCRED_DAP);
                    elencoAziendeVOCRED_DAP.Add(AziendaVOCRED_DAP);
                }
            }
        }

        public class DecAziendeVOCRED_DAP
        {
            public long Id { get; set; }
            public string TraduzioneSuGP { get; set; }
        }
    }
}
