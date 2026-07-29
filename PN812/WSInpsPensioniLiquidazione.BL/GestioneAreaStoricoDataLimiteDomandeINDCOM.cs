using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaStoricoDataLimiteDomandeINDCOM
    {
        public static void SalvaStorico (GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM datiStorico, out string messaggio)
        {
            messaggio = string.Empty;
            GestioneStoricoDataLimiteDomandeINDCOM.SalvaStoricoDataLimiteDomandeINDCOM(datiStorico);
        }

        public static void GetStoricoDataLimiteIDCOM(out List<GestioneStoricoDataLimiteDomandeINDCOM.DatiStoricoDataLimiteDomandeINDCOM> elencoStoricoDataLimiteINDCOM)
        {
            elencoStoricoDataLimiteINDCOM = null;
            GestioneStoricoDataLimiteDomandeINDCOM.GetStoricoDataLimiteINDCOM(out elencoStoricoDataLimiteINDCOM);
        }


        public static void UpdateNoteStoricoDataLimiteINDCOM(int id, string note, out string messaggio)
        {
            messaggio = string.Empty;
            GestioneStoricoDataLimiteDomandeINDCOM.UpdateNoteStoricoDataLimiteINDCOM(id, note);
        }


    }
}
