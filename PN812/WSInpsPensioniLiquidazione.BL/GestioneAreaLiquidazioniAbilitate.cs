using System;
using System.IO;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaLiquidazioniAbilitate
    {
        #region public members
        public static void GetAllLiquidazioniAbilitate(out List<GestioneLiquidazioniAbilitate.LiquidazioneAbilitata> elencoLiquidazioniAbilitate)
        {
            elencoLiquidazioniAbilitate = null;
            GestioneLiquidazioniAbilitate.GetAllLiquidazioniAbilitate(out elencoLiquidazioniAbilitate);
        }

        public static void StoreLiquidazioneAbilitata(GestioneLiquidazioniAbilitate.LiquidazioneAbilitata areaLiquidazioneAbilitata)
        {
            if (!GestioneLiquidazioniAbilitate.ControlSiglaCategoria(areaLiquidazioneAbilitata.SiglaCategoria, areaLiquidazioneAbilitata.Tipologia))
                throw new INPS.DNA.DnaValidationException("Sigla Categoria non valida");

            if (!GestioneLiquidazioniAbilitate.ControlSedeAmmessa(areaLiquidazioneAbilitata.Sede.HasValue ? areaLiquidazioneAbilitata.Sede.Value : (short)0))
                throw new INPS.DNA.DnaValidationException("Sede non valida");

            GestioneLiquidazioniAbilitate.SalvaLiquidazioneAbilitata(areaLiquidazioneAbilitata);
        }

        public static void StoreLiquidazioniAbilitateSuTutteLeSedi(GestioneLiquidazioniAbilitate.LiquidazioneAbilitata areaLiquidazioneAbilitata)
        {
            if (!GestioneLiquidazioniAbilitate.ControlSiglaCategoria(areaLiquidazioneAbilitata.SiglaCategoria, areaLiquidazioneAbilitata.Tipologia))
                throw new INPS.DNA.DnaValidationException("Sigla Categoria non valida");

            List<INPS.DNA.Office> elencoSediProvinciali = null;
            GestioneLiquidazioniAbilitate.GetSediAmmesse(out elencoSediProvinciali);
            if (elencoSediProvinciali != null && elencoSediProvinciali.Count > 0)
            {
                List<GestioneLiquidazioniAbilitate.LiquidazioneAbilitata> elencoLiquidazioniAbilitate = new List<GestioneLiquidazioniAbilitate.LiquidazioneAbilitata>();
                foreach (INPS.DNA.Office office in elencoSediProvinciali)
                {
                    GestioneLiquidazioniAbilitate.LiquidazioneAbilitata liqAb = new GestioneLiquidazioniAbilitate.LiquidazioneAbilitata();
                    liqAb.SiglaCategoria = areaLiquidazioneAbilitata.SiglaCategoria;
                    liqAb.Sede = Utility.StringToNullableShort(office.AspnCode.PadLeft(4, '0').Substring(0, 4));
                    liqAb.Tipologia = areaLiquidazioneAbilitata.Tipologia;
                    liqAb.Ricostituzione = areaLiquidazioneAbilitata.Ricostituzione;
                    liqAb.AbilitazioneManuale = areaLiquidazioneAbilitata.AbilitazioneManuale;
                    elencoLiquidazioniAbilitate.Add(liqAb);
                }
                GestioneLiquidazioniAbilitate.SalvaLiquidazioniAbilitateSuTutteLeSedi(elencoLiquidazioniAbilitate);
            }
        }


        public static void CancelLiquidazioneAbilitata(GestioneLiquidazioniAbilitate.LiquidazioneAbilitata areaLiquidazioneAbilitata)
        {
            if (!GestioneLiquidazioniAbilitate.ControlSiglaCategoria(areaLiquidazioneAbilitata.SiglaCategoria, areaLiquidazioneAbilitata.Tipologia))
                throw new INPS.DNA.DnaValidationException("Sigla Categoria non valida");
            
            GestioneLiquidazioniAbilitate.EliminaLiquidazioneAbilitata(areaLiquidazioneAbilitata);
        }
        #endregion public members
    }
}

