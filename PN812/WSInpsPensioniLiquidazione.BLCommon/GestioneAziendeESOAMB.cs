using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAziendeESOAMB
    {
        public static void GetDecodificaAziendeESOAMB(out List<DecAziendeESOAMB> elencoAziendeESOAMB)
        {
            elencoAziendeESOAMB = null;
            List<CtrlAziendaESOAMB> elencoDecodificaAziendeESOAMB = null;
            DAGestioneCtrlAziendaESOAMB.GetDecodificaAziendaESOAMB(out elencoDecodificaAziendeESOAMB);
            if (elencoDecodificaAziendeESOAMB != null && elencoDecodificaAziendeESOAMB.Count > 0)
            {
                elencoAziendeESOAMB = new List<DecAziendeESOAMB>();
                foreach (CtrlAziendaESOAMB decodificaAziendaESOAMB in elencoDecodificaAziendeESOAMB)
                {
                    DecAziendeESOAMB AziendaESOAMB = new DecAziendeESOAMB();
                    Utility.ValorizzaOggetti(decodificaAziendaESOAMB, AziendaESOAMB);
                    elencoAziendeESOAMB.Add(AziendaESOAMB);
                }
            }
        }

        public static void GetDecodificaAziendaESOAMBByIdCodiceAzienda(short idAzienda, out DecAziendeESOAMB aziendaESOAMB)
        {
            aziendaESOAMB = null;
            CtrlAziendaESOAMB aziendaESOAMBDatabase = null;
            DAGestioneCtrlAziendaESOAMB.GetDecodificaAziendaESOAMBByIdCodiceAzienda(idAzienda, out aziendaESOAMBDatabase);
            if (aziendaESOAMBDatabase != null)
            {
                aziendaESOAMB = new DecAziendeESOAMB();
                Utility.ValorizzaOggetti(aziendaESOAMBDatabase, aziendaESOAMB);
            }
        }

        public static void SalvaAziendeESOAMB(DecAziendeESOAMB decAziendaESOAMB, GestioneDecodificaAzienda.DecAzienda decAzienda) /*2 oggetti, del blcommon della classe innestata qui per aziende ESOAMB e del blcommon per aziende */
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab aziende ESOAMB con valorizzaoggetti
                CtrlAziendaESOAMB decodificaAziendaESOAMB = new CtrlAziendaESOAMB(); /*oggetto del datacommon per tab aziendeESOAMB*/
                Utility.ValorizzaOggetti(decAziendaESOAMB, decodificaAziendaESOAMB);

                //mapping per tab aziende con valorizzaoggetti
                DecodificaAzienda decodificaAzienda = new DecodificaAzienda();
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);

                DAGestioneCtrlAziendaESOAMB.InsertAziendeESOAMB(decodificaAziendaESOAMB, decodificaAzienda);/*salva 2 oggetti del datacommon nell'oggetto del datacommon*/

                transactionScope.Complete();
            }
        }

        public static void DeleteAziendaESOAMB(string codiceAziendaTraduzioneSuGP) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCtrlAziendaESOAMB.DeleteAziendaESOAMB(codiceAziendaTraduzioneSuGP);

                transactionScope.Complete();
            }
        }


        public class DecAziendeESOAMB
        {
            #region DecodificaAziende_CtrlAziendeESOAMB
            //CodiceAzienda a video = traduzione su GP
            //CodiceAzienda tabella CtrlAziendaESOAMB è FK con Id tabella DecodificaAzienda

            public long Id { get; set; }

            public short CodiceAzienda { get; set; }

            public DateTime? UltimaDecorrenzaAmmessa { get; set; }

            #endregion DecodificaAziende_CtrlAziendeESOAMB
        }
    }
}
