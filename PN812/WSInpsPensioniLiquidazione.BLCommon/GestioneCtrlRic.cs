using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlRic
    {
        #region public members
        public static void GetCtrlTabRic(string prodotto, Utility.TipoAppartenenza? tipoAppartenenza, out ControlTabRic controlTabRic)
        {
            controlTabRic = null;
            
            string tipologia = string.Empty;
            if (tipoAppartenenza.HasValue)
            {
                switch (tipoAppartenenza.Value)
                {
                    case Utility.TipoAppartenenza.AGO:
                        tipologia = "AGO";
                        break;
                    case Utility.TipoAppartenenza.CI:
                        tipologia = "CI";
                        break;
                    case Utility.TipoAppartenenza.FS:
                        tipologia = "FS";
                        break;
                }
                CtrlTabRic ctrlTabRic = null;
                DAGestioneCtrlRic.GetCtrlTabRic(prodotto, tipologia, out ctrlTabRic);
                if (ctrlTabRic != null)
                {
                    controlTabRic = new ControlTabRic();
                    Utility.ValorizzaOggetti(ctrlTabRic, controlTabRic);
                }
            }
        }
        #endregion public members

        #region nested classes
        public class ControlTabRic
        {
            #region private properties
            private bool _TabGenerici;
            private bool _TabAssicurativi;
            private bool _TabCalcolo;
            private bool _TabSupplementi;
            private bool _TabResEstero;
            #endregion private properties

            #region public properties
            public bool TabGenerici { get { return _TabGenerici; } set { _TabGenerici = value; } }
            public bool TabAssicurativi { get { return _TabAssicurativi; } set { _TabAssicurativi = value; } }
            public bool TabCalcolo { get { return _TabCalcolo; } set { _TabCalcolo = value; } }
            public bool TabSupplementi { get { return _TabSupplementi; } set { _TabSupplementi = value; } }
            public bool TabResEstero { get { return _TabResEstero; } set { _TabResEstero = value; } }
            #endregion public properties
        }
        #endregion nested classes
    }
}
