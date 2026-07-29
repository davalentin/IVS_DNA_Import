using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using INPS.DNA.Logging;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAziendeVESO33
    {
        public static void GetDecodificaAziendeVESO33(out List<DecAziendeVESO33> elencoAziendeVESO33)
        {
            elencoAziendeVESO33 = null;
            List<CtrlAziendaVESO33> elencoDecodificaAziendeVESO33DB = null;
            DAGestioneCtrlAziendeVESO33.GetDecodificaAziendeVESO33(out elencoDecodificaAziendeVESO33DB);
            if (elencoDecodificaAziendeVESO33DB != null && elencoDecodificaAziendeVESO33DB.Count > 0)
            {
                elencoAziendeVESO33 = new List<DecAziendeVESO33>();
                foreach (CtrlAziendaVESO33 decodificaAziendaVESO33DB in elencoDecodificaAziendeVESO33DB)
                {
                    DecAziendeVESO33 AziendaVESO33 = new DecAziendeVESO33();
                    Utility.ValorizzaOggetti(decodificaAziendaVESO33DB, AziendaVESO33);
                    elencoAziendeVESO33.Add(AziendaVESO33);
                }
            }
        }

        public static void GetDecodificaAziendaVESO33ByIdCodiceAzienda(short idAzienda, out DecAziendeVESO33 aziendaVESO33)
        {
            aziendaVESO33 = null;
            CtrlAziendaVESO33 aziendaVESO33Database = null;
            DAGestioneCtrlAziendeVESO33.GetDecodificaAziendaVESO33ByIdCodiceAzienda(idAzienda, out aziendaVESO33Database);
            if (aziendaVESO33Database != null)
            {
                aziendaVESO33 = new DecAziendeVESO33();
                Utility.ValorizzaOggetti(aziendaVESO33Database, aziendaVESO33);
            }
        }

        public static void SalvaAziendeVESO33(DecAziendeVESO33 decAziendaVESO33, GestioneDecodificaAzienda.DecAzienda decAzienda) /*2 oggetti, del blcommon della classe innestata qui per aziende veso 33 e del blcommon per aziende */
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab aziende veso33 con valorizzaoggetti
                DataCommon.CtrlAziendaVESO33 decodificaAziendaVESO33 = new CtrlAziendaVESO33();/*oggetto del datacommon per tab aziendeVESO33*/
                Utility.ValorizzaOggetti(decAziendaVESO33, decodificaAziendaVESO33);

                //mapping per tab aziende con valorizzaoggetti
                DataCommon.DecodificaAzienda decodificaAzienda = new DecodificaAzienda();/*oggetto del datacommon per tab aziendeVESO33*/
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);

                DAGestioneCtrlAziendeVESO33.InsertAziendeVESO33(decodificaAziendaVESO33, decodificaAzienda);/*salva 2 oggetti del datacommon nell'oggetto del datacommon*/

                transactionScope.Complete();
            }

        }

        public static void DeleteAziendaVESO33(string codiceAziendaTraduzioneSuGP)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {

                DAGestioneCtrlAziendeVESO33.DeleteAziendaVESO33(codiceAziendaTraduzioneSuGP);
                
                transactionScope.Complete();
            }
        }


        public class DecAziendeVESO33
        {
            #region DecodificaAziende_CtrlAziendeVESO33
            //CodiceAzienda a video = traduzione su GP
            //CodiceAzienda tabella CtrlVESO33 è FK con Id tabella DecodificaAzienda

            public long Id { get; set; }
            public short CodiceAzienda { get; set; }
            public DateTime? UltimaDecorrenzaAmmessa { get; set; }

            #endregion DecodificaAziende_CtrlAziendeVESO33
        }
    }
}
