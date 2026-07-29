using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlStatiSenzaEsenzioneFiscaleEstera
    {
        public static void GetStatoSenzaEsenzioneFiscaleEstera(string codCatastale, out DatiCtrlStatiSenzaEsenzioneFiscaleEstera statoSenzaEsenzioneFiscaleEstera)
        {
            statoSenzaEsenzioneFiscaleEstera = null;

            CtrlStatiSenzaEsenzioneFiscaleEstera ctrl = null;
            DAGestioneCtrlStatiSenzaEsenzioneFiscaleEstera.GetCtrlStatiSenzaEsenzioneFiscaleEstera(codCatastale, out ctrl);

            if (ctrl != null)
            {
                statoSenzaEsenzioneFiscaleEstera = new DatiCtrlStatiSenzaEsenzioneFiscaleEstera();
                Utility.ValorizzaOggetti(ctrl, statoSenzaEsenzioneFiscaleEstera);
            }
        }

        public static void GetStatoEsenzioneFiscaleEsteraINPDAP(string codCatastale, out DatiCtrlStatiSenzaEsenzioneFiscaleEstera statoEsenzioneFiscaleEsteraINPDAP)
        {
            statoEsenzioneFiscaleEsteraINPDAP = null;

            CtrlStatiEsenzioneFiscaleEsteraINPDAP ctrl = null;
            DAGestioneCtrlStatiEsenzioneFiscaleEsteraINPDAP.GetCtrlStatiEsenzioneFiscaleEsteraINPDAP(codCatastale, out ctrl);

            if (ctrl != null)
            {
                statoEsenzioneFiscaleEsteraINPDAP = new DatiCtrlStatiSenzaEsenzioneFiscaleEstera();
                Utility.ValorizzaOggetti(ctrl, statoEsenzioneFiscaleEsteraINPDAP);
            }
        }
    }

    public class DatiCtrlStatiSenzaEsenzioneFiscaleEstera
    {
        public string CodCatastale { get; set;}
    }
}
