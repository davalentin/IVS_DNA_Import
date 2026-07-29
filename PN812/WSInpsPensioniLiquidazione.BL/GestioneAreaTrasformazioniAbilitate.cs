using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaTrasformazioniAbilitate
    {
        #region public members
        public static void GetAllTrasformazioniAbilitate(out List<GestioneTrasformazioniAbilitate.TrasformazioneAbilitata> elencoTrasformazioniAbilitate)
        {
            elencoTrasformazioniAbilitate = null;
            GestioneTrasformazioniAbilitate.GetAllTrasformazioniAbilitate(out elencoTrasformazioniAbilitate);
        }

        public static void StoreTrasformazioneAbilitata(GestioneTrasformazioniAbilitate.TrasformazioneAbilitata areaTrasformazioneAbilitata)
        {
            if (!GestioneTrasformazioniAbilitate.ControlSiglaCategoria(areaTrasformazioneAbilitata.SiglaCategoria, areaTrasformazioneAbilitata.Tipologia))
                throw new INPS.DNA.DnaValidationException("Sigla Categoria non valida");

            if (!GestioneTrasformazioniAbilitate.ControlSedeAmmessa(areaTrasformazioneAbilitata.Sede.HasValue ? areaTrasformazioneAbilitata.Sede.Value : (short)0))
                throw new INPS.DNA.DnaValidationException("Sede non valida");

            GestioneTrasformazioniAbilitate.SalvaTrasformazioneAbilitata(areaTrasformazioneAbilitata);
        }

        public static void StoreTrasaformazioniAbilitateSuTutteLeSedi(GestioneTrasformazioniAbilitate.TrasformazioneAbilitata areaTrasformazioneAbilitata)
        {
            if (!GestioneTrasformazioniAbilitate.ControlSiglaCategoria(areaTrasformazioneAbilitata.SiglaCategoria, areaTrasformazioneAbilitata.Tipologia))
                throw new INPS.DNA.DnaValidationException("Sigla Categoria non valida");

            List<INPS.DNA.Office> elencoSediProvinciali = null;
            GestioneTrasformazioniAbilitate.GetSediAmmesse(out elencoSediProvinciali);
            if (elencoSediProvinciali != null && elencoSediProvinciali.Count > 0)
            {
                List<GestioneTrasformazioniAbilitate.TrasformazioneAbilitata> elencoTrasformazioniAbilitate = new List<GestioneTrasformazioniAbilitate.TrasformazioneAbilitata>();
                foreach (INPS.DNA.Office office in elencoSediProvinciali)
                {
                    GestioneTrasformazioniAbilitate.TrasformazioneAbilitata traAb = new GestioneTrasformazioniAbilitate.TrasformazioneAbilitata();
                    traAb.SiglaCategoria = areaTrasformazioneAbilitata.SiglaCategoria;
                    traAb.Sede = Utility.StringToNullableShort(office.AspnCode.PadLeft(4, '0').Substring(0, 4));
                    traAb.Tipologia = areaTrasformazioneAbilitata.Tipologia;
                    elencoTrasformazioniAbilitate.Add(traAb);
                }
                GestioneTrasformazioniAbilitate.SalvaTrasformazioniAbilitateSuTutteLeSedi(elencoTrasformazioniAbilitate);
            }
        }


        public static void CancelTrasformazioneAbilitata(GestioneTrasformazioniAbilitate.TrasformazioneAbilitata areaTrasformazioneAbilitata)
        {
            if (!GestioneTrasformazioniAbilitate.ControlSiglaCategoria(areaTrasformazioneAbilitata.SiglaCategoria, areaTrasformazioneAbilitata.Tipologia))
                throw new INPS.DNA.DnaValidationException("Sigla Categoria non valida");

            GestioneTrasformazioniAbilitate.EliminaTrasformazioneAbilitata(areaTrasformazioneAbilitata);
        }
        #endregion public members
    }
}
