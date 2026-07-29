using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public static class GestioneCtrlCodiciFiscaliAbilitatiPerTipo0179
    {
        public static void GetAbilitazionePerTipo0179byCodiceFiscale(string codiceFiscale, out CtrlCodiciFiscaliAbilitatiPerTipo0179 ctrlCodiciFiscaliAbilitatiPerTipo0179)
        {
            ctrlCodiciFiscaliAbilitatiPerTipo0179 = null;
            DataCommon.CtrlCodiciFiscaliAbilitatiPerTipo0179 ctrlCodiciFiscaliFromDb = null;
            DAGestioneCtrlCodiciFiscaliAbilitatiPerTipo0179.GetCtrlCodiciFiscaliAbilitatiPerTipo0179byCodiceFiscale(codiceFiscale, out ctrlCodiciFiscaliFromDb);

            if (ctrlCodiciFiscaliFromDb != null)
            {
                ctrlCodiciFiscaliAbilitatiPerTipo0179 = new CtrlCodiciFiscaliAbilitatiPerTipo0179();
                Utility.ValorizzaOggetti(ctrlCodiciFiscaliFromDb, ctrlCodiciFiscaliAbilitatiPerTipo0179);
            }
        }
    }

    #region CtrlCodiciFiscaliAbilitatiPerTipo0179
    public class CtrlCodiciFiscaliAbilitatiPerTipo0179
    {
        public long Id { get; set; }

        public long IdDecAnagraficaAccordi0179 { get; set; }

        public string CodiceFiscale { get; set; }

        public DateTime TimeStamp { get; set; }
    }
    #endregion CtrlCodiciFiscaliAbilitatiPerTipo0179
}
