using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlBypassTipologieNonAbilitate
    {
        public static bool IsDomandaLavorabilePerEccezione(Utility.TipoAppartenenza? tipologia, short sede, string gruppo, string prod, string tipo, string categoria, string codiceTipoRichiesta, bool isINPDAP)
        {
            List<DataCommon.CtrlBypassTipologieNonAbilitate> ctrlBypassTipologieNonAbilitate = new List<DataCommon.CtrlBypassTipologieNonAbilitate>();
            string filtro = string.Empty;
            string fondo = null;

            #region Get Filtro
            if (codiceTipoRichiesta == null)
                filtro = "ALL";
            else
                filtro = Utility.GetFiltroByCodTipoRichiesta(codiceTipoRichiesta);

            if (filtro == null)
                filtro = string.Empty;
            #endregion Get Filtro

            #region Get Fondo
            if (tipologia == Utility.TipoAppartenenza.FS)
            {
                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(false, "007", categoria);
                fondo = isINPDAP ? "INPDAP" : tipoFondo.HasValue ? tipoFondo.Value.ToString().Trim() : null;
            }
            #endregion Get Fondo

            DAGestioneCtrlBypassTipologieNonAbilitate.GetCtrlBypassTipologieNonAbilitate(tipologia.ToString(), sede, gruppo, prod, tipo, categoria, filtro, fondo, out ctrlBypassTipologieNonAbilitate);

            if (ctrlBypassTipologieNonAbilitate != null && ctrlBypassTipologieNonAbilitate.Count > 0)
                return true;

            return false;
        }

        public static void GetCtrlBypassTipologieNonAbilitate(out List<CtrlBypassTipologieNonAbilitate> elencoCtrlBypassTipologieNonAbilitate)
        {
            elencoCtrlBypassTipologieNonAbilitate = null;
            List<DataCommon.CtrlBypassTipologieNonAbilitate> elencoCtrlBypassTipologieNonAbilitateDB = null;
            DAGestioneCtrlBypassTipologieNonAbilitate.GetCtrlBypassTipologieNonAbilitate(out elencoCtrlBypassTipologieNonAbilitateDB);
            if (elencoCtrlBypassTipologieNonAbilitateDB != null && elencoCtrlBypassTipologieNonAbilitateDB.Count > 0)
            {
                elencoCtrlBypassTipologieNonAbilitate = new List<CtrlBypassTipologieNonAbilitate>();
                foreach (DataCommon.CtrlBypassTipologieNonAbilitate ctrlDB in elencoCtrlBypassTipologieNonAbilitateDB)
                {
                    CtrlBypassTipologieNonAbilitate ctrl = new CtrlBypassTipologieNonAbilitate();
                    Utility.ValorizzaOggetti(ctrlDB, ctrl);
                    elencoCtrlBypassTipologieNonAbilitate.Add(ctrl);
                }
            }
        }

        public static void SalvaCtrlBypassTipologieNonAbilitate(CtrlBypassTipologieNonAbilitate ctrlBypassTipologieNonAbilitate)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.CtrlBypassTipologieNonAbilitate ctrl = new DataCommon.CtrlBypassTipologieNonAbilitate();
                Utility.ValorizzaOggetti(ctrlBypassTipologieNonAbilitate, ctrl);
                DAGestioneCtrlBypassTipologieNonAbilitate.StoreCtrlBypassTipologieNonAbilitate(ctrl);
                transactionScope.Complete();
            }
        }

        public static void EliminaCtrlBypassTipologieNonAbilitate(CtrlBypassTipologieNonAbilitate ctrlBypassTipologieNonAbilitate)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DataCommon.CtrlBypassTipologieNonAbilitate ctrl = new DataCommon.CtrlBypassTipologieNonAbilitate();
                Utility.ValorizzaOggetti(ctrlBypassTipologieNonAbilitate, ctrl);
                DAGestioneCtrlBypassTipologieNonAbilitate.EliminaCtrlBypassTipologieNonAbilitate(ctrl);
                transactionScope.Complete();
            }
        }

        public class CtrlBypassTipologieNonAbilitate
        {
            public string Tipologia { get; set; }
            public short Sede { get; set; }
            public string Gruppo { get; set; }
            public string Prodotto { get; set; }
            public string Tipo { get; set; }
            public string Categoria { get; set; }
            public string Filtro { get; set; }
            public string Fondo { get; set; }
        }
    }
}
