using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaProvvisoriePerCoefficienti
    {
        public static void GetDecorrenzaProvvisoriaObbligatoriaPerTipoAppartenenza(Utility.TipoAppartenenza? tipoAppartenenza, out DateTime? dataDecorrenzaProvvisoriaObbligatoriaToGet)
        {
            dataDecorrenzaProvvisoriaObbligatoriaToGet = null;
            GestioneControlliDinamici.GetDecorrenzaProvvisoriaObbligatoria(tipoAppartenenza, out dataDecorrenzaProvvisoriaObbligatoriaToGet);
        }

        public static void SetDecorrenzaProvvisoriaObbligatoriaPerTipoAppartenenza(Utility.TipoAppartenenza? tipoAppartenenza, DateTime? dataDecorrenzaProvvisoriaObbligatoriaToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!ControlliDataProvvisoriaObbligatoria(dataDecorrenzaProvvisoriaObbligatoriaToSave, out messaggioVideo))
                return;
            else
                GestioneControlliDinamici.SetDecorrenzaProvvisoriaObbligatoria(tipoAppartenenza, dataDecorrenzaProvvisoriaObbligatoriaToSave);
        }

        private static bool ControlliDataProvvisoriaObbligatoria(DateTime? dataDecorrProvvObblToSave, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            //controllo obbligatorietà della Data Decorrenza Provvisoria Obbligatoria
            if (!dataDecorrProvvObblToSave.HasValue)
            {
                messaggioVideo = "E' necessario inserire la data Decorrenza Provvisoria Obbligatoria";
                return false;
            }

            // controllo data Decorrenza Provvisoria Obbligatoria successiva a gennaio 2017
            if (!Utility.DataSuccessivaA(dataDecorrProvvObblToSave.Value, new DateTime(2017, 1, 1)))
            {
                messaggioVideo = "La data Decorrenza Provvisoria Obbligatoria deve essere successiva al 01/01/2017";
                return false;
            }
            return true;
        }
    }
}
