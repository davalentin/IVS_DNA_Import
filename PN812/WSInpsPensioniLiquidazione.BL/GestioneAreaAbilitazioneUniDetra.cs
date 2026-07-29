using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAbilitazioneUniDetra
    {
        public static void GetAbilitazioneUniDetra(out bool isUniDetraAttivo)
        {
            isUniDetraAttivo = GestioneControlliDinamici.IsServizioUniDetraAttivo();
        }

        public static void SetAbilitazioneUniDetra(bool isUniDetraattivo, out string messaggioVideo)
        {
            messaggioVideo = null;
            GestioneControlliDinamici.ControlloDinamico controllodinamico = new GestioneControlliDinamici.ControlloDinamico();
            controllodinamico.NomeControllo = "ServizioUniDetraAttivo";
            if (isUniDetraattivo == true)
                controllodinamico.ValoreControllo = "SI";
            else
                controllodinamico.ValoreControllo = "NO";
            GestioneControlliDinamici.SalvaControlloDinamico(controllodinamico);
        }
    }
}
