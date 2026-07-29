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
    public class GestioneAziendeVESO29
    {
        public static void GetDecodificaAziendeVESO29(out List<DecAziendeVESO29> elencoAziendeVESO29)
        {
            elencoAziendeVESO29 = null;
            List<CtrlAziendaVESO29> elencoDecodificaAziendeVESO29DB = null;
            DAGestioneCtrlAziendeVESO29.GetDecodificaAziendeVESO29(out elencoDecodificaAziendeVESO29DB);
            if (elencoDecodificaAziendeVESO29DB != null && elencoDecodificaAziendeVESO29DB.Count > 0)
            {
                elencoAziendeVESO29 = new List<DecAziendeVESO29>();
                foreach (CtrlAziendaVESO29 decodificaAziendaVESO29DB in elencoDecodificaAziendeVESO29DB)
                {
                    DecAziendeVESO29 AziendaVESO29 = new DecAziendeVESO29();
                    Utility.ValorizzaOggetti(decodificaAziendaVESO29DB, AziendaVESO29);
                    elencoAziendeVESO29.Add(AziendaVESO29);
                }
            }
        }

        public static void GetDecodificaAziendaVESO29ByIdCodiceAzienda(short idAzienda, out DecAziendeVESO29 aziendaVESO29)
        {
            aziendaVESO29 = null;
            CtrlAziendaVESO29 aziendaVESO29Database = null;
            DAGestioneCtrlAziendeVESO29.GetDecodificaAziendaVESO29ByIdCodiceAzienda(idAzienda, out aziendaVESO29Database);
            if (aziendaVESO29Database != null)
            {
                aziendaVESO29 = new DecAziendeVESO29();
                Utility.ValorizzaOggetti(aziendaVESO29Database, aziendaVESO29);
            }
        }

        public static void SalvaAziendeVESO29(DecAziendeVESO29 decAziendaVESO29, GestioneDecodificaAzienda.DecAzienda decAzienda) /*2 oggetti, del blcommon della classe innestata qui per aziende veso 29 e del blcommon per aziende */
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab aziende veso29 con valorizzaoggetti
                DataCommon.CtrlAziendaVESO29 decodificaAziendaVESO29 = new CtrlAziendaVESO29();/*oggetto del datacommon per tab aziendeVESO29*/
                Utility.ValorizzaOggetti(decAziendaVESO29, decodificaAziendaVESO29);

                //mapping per tab aziende con valorizzaoggetti
                DataCommon.DecodificaAzienda decodificaAzienda = new DecodificaAzienda();/*oggetto del datacommon per tab aziendeVESO29*/
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);

                DAGestioneCtrlAziendeVESO29.InsertAziendeVESO29(decodificaAziendaVESO29, decodificaAzienda);/*salva 2 oggetti del datacommon nell'oggetto del datacommon*/

                transactionScope.Complete();
            }

        }

        public static void DeleteAziendaVESO29(string codiceAziendaTraduzioneSuGP)/*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {

                DAGestioneCtrlAziendeVESO29.DeleteAziendaVESO29(codiceAziendaTraduzioneSuGP);

                transactionScope.Complete();
            }
        }


        public class DecAziendeVESO29
        {
            #region DecodificaAziende_CtrlAziendeVESO29
            //CodiceAzienda a video = traduzione su GP
            //CodiceAzienda tabella CtrlVESO29 è FK con Id tabella DecodificaAzienda

            public long Id { get; set; }
            public short CodiceAzienda { get; set; }
            public DateTime? UltimaDecorrenzaAmmessa { get; set; }

            #endregion DecodificaAziende_CtrlAziendeVESO29
        }
    }
}
