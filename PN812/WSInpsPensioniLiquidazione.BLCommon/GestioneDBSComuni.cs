using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDBSComuni
    {
        #region public members
        public static void GetCodInpsComuneByCodCatastale(string codiceCatastale, string tipoAppartenenza, int codiceComuneInpsDaConfrontare, bool isPrelievoFS, out int codInpsComune)
        {
            codInpsComune = 0;
            DAGestioneDBSComuni.GetCodInpsComuneByCodCatastale(codiceCatastale, tipoAppartenenza, codiceComuneInpsDaConfrontare, isPrelievoFS, out codInpsComune);
        }
        #endregion public members
    }
}
