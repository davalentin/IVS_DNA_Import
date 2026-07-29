using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAvvisi
    {
        #region public methods
        /// <summary>
        /// Ritorna l'elenco degli avvisi
        /// </summary>
        /// <param name="elencoAvvisi">Lista avvisi</param>
        public static void GetAvvisi(bool getAvvisiAttivi, string tipoApp, out List<Avvisi> elencoAvvisi)
        {
            elencoAvvisi = null;
            List<BLCommon.GestioneAvvisi.DatiAvvisi> elencoAvvisiBL = null;
            if (getAvvisiAttivi)
                BLCommon.GestioneAvvisi.GetAvvisiAttivi(tipoApp, out elencoAvvisiBL);
            else
                BLCommon.GestioneAvvisi.GetAllAvvisi(tipoApp, out elencoAvvisiBL);

            if (elencoAvvisiBL != null)
            {
                elencoAvvisi = new List<Avvisi>();
                foreach (BLCommon.GestioneAvvisi.DatiAvvisi aBL in elencoAvvisiBL)
                {
                    Avvisi aBLComplex = new Avvisi();
                    Utility.ValorizzaOggetti(aBL, aBLComplex);
                    elencoAvvisi.Add(aBLComplex);
                }
            }
        }

        /// <summary>
        /// Salva avviso
        /// </summary>
        /// <param name="avviso">avviso</param>
        public static void StoreAvviso(Avvisi avviso)
        {
            avviso.TimeStamp = DateTime.Now;
            BLCommon.GestioneAvvisi.DatiAvvisi avvisoBL = new BLCommon.GestioneAvvisi.DatiAvvisi();
            Utility.ValorizzaOggetti(avviso, avvisoBL);
            BLCommon.GestioneAvvisi.SalvaAvviso(avvisoBL);
        }

        /// <summary>
        /// Cancella avviso
        /// </summary>
        /// <param name="avviso">avviso</param>
        public static void DeleteAvviso(Avvisi avviso)
        {
            BLCommon.GestioneAvvisi.DeleteAvviso(avviso.Id);
        }
        #endregion public methods
    }
}
