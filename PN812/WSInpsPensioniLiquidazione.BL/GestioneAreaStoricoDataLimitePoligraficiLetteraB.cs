using INPS.Pensioni.Liquidazione.BLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaStoricoDataLimiteDomandePoligraficiLetteraB
    {
        public static void SalvaStorico(GestioneStoricoDataLimitePoligraficiLetteraB.DatiStoricoDataLimitePoligraficiLetteraB datiStorico, out string messaggio)
        {
            messaggio = string.Empty;
            GestioneStoricoDataLimitePoligraficiLetteraB.SalvaStoricoDataLimitePoligraficiLetteraB(datiStorico);
        }

        public static void GetStoricoDataLimitePoligraficiLetteraB(out List<GestioneStoricoDataLimitePoligraficiLetteraB.DatiStoricoDataLimitePoligraficiLetteraB> elencoStoricoDataLimitePoligraficiLetteraB)
        {
            elencoStoricoDataLimitePoligraficiLetteraB = null;
            GestioneStoricoDataLimitePoligraficiLetteraB.GetStoricoDataLimitePoligraficiLetteraB(out elencoStoricoDataLimitePoligraficiLetteraB);
        }


        public static void UpdateNoteStoricoDataLimitePoligraficiLetteraB(int id, string note, out string messaggio)
        {
            messaggio = string.Empty;
            GestioneStoricoDataLimitePoligraficiLetteraB.UpdateNoteStoricoDataLimitePoligraficiLetteraB(id, note);
        }
    }
}
