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
    public class GestioneAziendeCredito
    {
        public static void GetDecodificaAziendeCredito(out List<DecAziendeCredito> elencoAziendeCredito)
        {
            elencoAziendeCredito = null;
            List<CtrlAziendaCredito> elencoDecodificaAziendeCredito = null;
            DAGestioneCtrlAziendaCredito.GetDecodificaAziendaCredito(out elencoDecodificaAziendeCredito);
            if (elencoDecodificaAziendeCredito != null && elencoDecodificaAziendeCredito.Count > 0)
            {
                elencoAziendeCredito = new List<DecAziendeCredito>();
                foreach (CtrlAziendaCredito decodificaAziendaCredito in elencoDecodificaAziendeCredito)
                {
                    DecAziendeCredito AziendaCredito = new DecAziendeCredito();
                    Utility.ValorizzaOggetti(decodificaAziendaCredito, AziendaCredito);
                    elencoAziendeCredito.Add(AziendaCredito);
                }
            }
        }

        public static void GetDecodificaAziendaCreditoByIdCodiceAzienda(short idAzienda, out DecAziendeCredito aziendaCredito)
        {
            aziendaCredito = null;
            CtrlAziendaCredito aziendaCreditoDatabase = null;
            DAGestioneCtrlAziendaCredito.GetDecodificaAziendaCreditoByIdCodiceAzienda(idAzienda, out aziendaCreditoDatabase);
            if (aziendaCreditoDatabase != null)
            {
                aziendaCredito = new DecAziendeCredito();
                Utility.ValorizzaOggetti(aziendaCreditoDatabase, aziendaCredito);
            }
        }

        public static void SalvaAziendeCredito(DecAziendeCredito decAziendaCredito, GestioneDecodificaAzienda.DecAzienda decAzienda) /*2 oggetti, del blcommon della classe innestata qui per aziende credito e del blcommon per aziende */
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab aziende credito con valorizzaoggetti
                DataCommon.CtrlAziendaCredito decodificaAziendaCredito = new CtrlAziendaCredito(); /*oggetto del datacommon per tab aziendeCredito*/
                Utility.ValorizzaOggetti(decAziendaCredito, decodificaAziendaCredito);

                //mapping per tab aziende con valorizzaoggetti
                DataCommon.DecodificaAzienda decodificaAzienda = new DecodificaAzienda();
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);

                DAGestioneCtrlAziendaCredito.InsertAziendeCredito(decodificaAziendaCredito, decodificaAzienda);/*salva 2 oggetti del datacommon nell'oggetto del datacommon*/

                transactionScope.Complete();
            }
        }

        public static void DeleteAziendaCredito(string codiceAziendaTraduzioneSuGP) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {

                DAGestioneCtrlAziendaCredito.DeleteAziendaCredito(codiceAziendaTraduzioneSuGP);

                transactionScope.Complete();
            }
        }


        public class DecAziendeCredito
        {
            #region DecodificaAziende_CtrlAziendeCredito
            //CodiceAzienda a video = traduzione su GP
            //CodiceAzienda tabella CtrlAziendaCredito è FK con Id tabella DecodificaAzienda

            public long Id { get; set; }
            
            public short CodiceAzienda { get; set; }

            public DateTime? UltimaDecorrenzaAmmessa { get; set; }

            public string SiglaCatPensione { get; set; }

            #endregion DecodificaAziende_CtrlAziendeCredito
        }
    }
}
