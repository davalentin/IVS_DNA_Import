using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlStatiSenzaEsenzioneEsteraAutonomi
    {
        public static void GetListaStatiSenzaEsenzioneEsteraAutonomi(out List<DatiCtrlStatiSenzaEsenzioneEsteraAutonomi> listaStati)
        {
            listaStati = null;

            List<CtrlStatiSenzaEsenzioneFiscaleEsteraPerAutonomi> listaStatiCtrl = null;
            DAGestioneCtrlStatiSenzaEsenzioneEsteraPerAutonomi.GetListaStatiSenzaEsenzione(out listaStatiCtrl);

            if (listaStatiCtrl != null && listaStatiCtrl.Count > 0)
            {
                listaStati = new List<DatiCtrlStatiSenzaEsenzioneEsteraAutonomi>();
                foreach (CtrlStatiSenzaEsenzioneFiscaleEsteraPerAutonomi statoCtrl in listaStatiCtrl)
                {
                    DatiCtrlStatiSenzaEsenzioneEsteraAutonomi stato = new DatiCtrlStatiSenzaEsenzioneEsteraAutonomi();
                    Utility.ValorizzaOggetti(statoCtrl, stato);
                    listaStati.Add(stato);
                }
            }
        }
    }

    public class DatiCtrlStatiSenzaEsenzioneEsteraAutonomi
    {
        public string CodCatastale { get; set;}
    }
}
