using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA.Logging;
using System.Transactions;
using System.Linq.Expressions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneDecodificaAzienda
    {

        /// <summary>
        /// inserimento aziende
        /// metodo del BL common, richiama metodo del Data common
        /// </summary>
        /// <param name="decAzienda"></param>
        public static void InsertDecodificaAzienda(DecAzienda decAzienda)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
             new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DecodificaAzienda decodificaAzienda = new DecodificaAzienda();/*oggetto del datacommon*/
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);
                DAGestioneDecodificaAzienda.InsertDecodificaAzienda(decodificaAzienda);
                transactionScope.Complete();
            }
        }

        public static void GetElencoAziendaBySiglaCategoria(string siglaCategoria, string tipo, out List<DecAzienda> elencoAziendaEditoria)
        {
            elencoAziendaEditoria = new List<DecAzienda>();
            List<DecodificaAzienda> elencoAziendaEditoriaDB = null;

            Expression<Func<DecodificaAzienda, bool>> whereCondition = p => true;

            if (Utility.IsDomandaVOESO(siglaCategoria))
            {
                whereCondition = whereCondition.And(x => x.SiglaCategoria == "VOESO");
                if(tipo!=null)
                    whereCondition = whereCondition.And(x => x.Tipo == tipo);
            }               
            else if (Utility.IsDomandaVESO33(siglaCategoria) || Utility.IsDomandaVESO92(siglaCategoria) || Utility.IsDomandaVOCRED(siglaCategoria) || Utility.IsDomandaVOCOOP(siglaCategoria) ||
                     Utility.IsDomandaVESO29(siglaCategoria) || Utility.IsDomandaESOTEL(siglaCategoria) || Utility.IsDomandaESOAMB(siglaCategoria) || Utility.IsDomandaESPA(siglaCategoria) ||
                     Utility.IsDomandaESOPMI(siglaCategoria))
                whereCondition = whereCondition.And(x => x.SiglaCategoria == siglaCategoria.Trim().ToUpperInvariant());
            else if (Utility.IsDomandaCRED27(siglaCategoria))
                whereCondition = whereCondition.And(x => x.SiglaCategoria == "VOCRED");
            else if (Utility.IsDomandaCOOP28(siglaCategoria))
                whereCondition = whereCondition.And(x => x.SiglaCategoria == "VOCOOP");
            else
                whereCondition = whereCondition.And(x => x.SiglaCategoria == null);

            DAGestioneDecodificaAzienda.GetDecodificaAziendaBySiglaCategoria(whereCondition, out elencoAziendaEditoriaDB);

            foreach (DecodificaAzienda AziendaEditoriaDB in elencoAziendaEditoriaDB)
            {
                DecAzienda decAziendaEditoriaBL = new DecAzienda();
                Utility.ValorizzaOggetti(AziendaEditoriaDB, decAziendaEditoriaBL);
                elencoAziendaEditoria.Add(decAziendaEditoriaBL);
            }
        }

        public static DecAzienda GetAziendaById(short id)
        {
            DecodificaAzienda decAziendaEditoriaDB = null;
            decAziendaEditoriaDB = DAGestioneDecodificaAzienda.GetDecodificaAziendaById(id);
            DecAzienda decAziendaEditoriaBL = new DecAzienda();
            Utility.ValorizzaOggetti(decAziendaEditoriaDB, decAziendaEditoriaBL);
            return decAziendaEditoriaBL;
        }
		
        public static void GetElencoAziendaAll(out List<DecAzienda> elencoAziendaEditoria)
        {
            elencoAziendaEditoria = null;
            List<DecodificaAzienda> elencoAziendaEditoriaDB = null;
            DAGestioneDecodificaAzienda.GetDecodificaAziendaAll(out elencoAziendaEditoriaDB);

            if (elencoAziendaEditoriaDB != null && elencoAziendaEditoriaDB.Count > 0)
                elencoAziendaEditoria = elencoAziendaEditoriaDB.Select(x => { var decBl = new DecAzienda(); Utility.ValorizzaOggetti(x, decBl); return decBl; }).ToList();
            else
                elencoAziendaEditoria = new List<DecAzienda>();
        }

        #region nested class
        public class DecAzienda
        {
            #region public properties

            public short Id { get { return _Id; } set { _Id = value; } }
            public string TraduzioneSuGP { get { return _TraduzioneSuGP; } set { _TraduzioneSuGP = value; } }
            public string Descrizione { get { return _Descrizione; } set { _Descrizione = value; } }
            public string SiglaCategoria { get; set; }
            public string Tipo { get; set; }
            #endregion public properties

            #region private properties
            private short _Id;
            private string _TraduzioneSuGP;
            private string _Descrizione;
            #endregion private properties
        }
        #endregion nested class
    }
}
