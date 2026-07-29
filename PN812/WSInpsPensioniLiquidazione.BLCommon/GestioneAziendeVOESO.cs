using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAziendeVOESO
    {
        public static void GetDecodificaAziendeVOESO(out List<DecAziendeVOESO> elencoAziendeVOESO)
        {
            elencoAziendeVOESO = null;
            List<CtrlAziendaVOESO> elencoDecodificaAziendeVOESO = null;
            DAGestioneCtrlAziendaVOESO.GetDecodificaAziendaVOESO(out elencoDecodificaAziendeVOESO);
            if (elencoDecodificaAziendeVOESO != null && elencoDecodificaAziendeVOESO.Count > 0)
            {
                elencoAziendeVOESO = new List<DecAziendeVOESO>();
                foreach (CtrlAziendaVOESO decodificaAziendaVOESO in elencoDecodificaAziendeVOESO)
                {
                    DecAziendeVOESO AziendaVOESO = new DecAziendeVOESO();
                    Utility.ValorizzaOggetti(decodificaAziendaVOESO, AziendaVOESO);
                    elencoAziendeVOESO.Add(AziendaVOESO);
                }
            }
        }

        public static void GetDecodificaAziendaVOESOByIdCodiceAzienda(short idAzienda, out DecAziendeVOESO aziendaVOESO)
        {
            aziendaVOESO = null;
            CtrlAziendaVOESO aziendaVOESODatabase = null;
            DAGestioneCtrlAziendaVOESO.GetDecodificaAziendaVOESOByIdCodiceAzienda(idAzienda, out aziendaVOESODatabase);
            if (aziendaVOESODatabase != null)
            {
                aziendaVOESO = new DecAziendeVOESO();
                Utility.ValorizzaOggetti(aziendaVOESODatabase, aziendaVOESO);
            }
        }

        public static void SalvaAziendeVOESO(DecAziendeVOESO decAziendaVOESO, GestioneDecodificaAzienda.DecAzienda decAzienda) /*2 oggetti, del blcommon della classe innestata qui per aziende VOESO e del blcommon per aziende */
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab aziende VOESO con valorizzaoggetti
                CtrlAziendaVOESO decodificaAziendaVOESO = new CtrlAziendaVOESO(); /*oggetto del datacommon per tab aziendeVOESO*/
                Utility.ValorizzaOggetti(decAziendaVOESO, decodificaAziendaVOESO);

                //mapping per tab aziende con valorizzaoggetti
                DecodificaAzienda decodificaAzienda = new DecodificaAzienda();
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);

                DAGestioneCtrlAziendaVOESO.InsertAziendeVOESO(decodificaAziendaVOESO, decodificaAzienda);/*salva 2 oggetti del datacommon nell'oggetto del datacommon*/

                transactionScope.Complete();
            }
        }

        public static void DeleteAziendaVOESO(string codiceAziendaTraduzioneSuGP, string tipo) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCtrlAziendaVOESO.DeleteAziendaVOESO(codiceAziendaTraduzioneSuGP, tipo);

                transactionScope.Complete();
            }
        }


        public class DecAziendeVOESO
        {
            #region DecodificaAziende_CtrlAziendeVOESO
            //CodiceAzienda a video = traduzione su GP
            //CodiceAzienda tabella CtrlAziendaVOESO è FK con Id tabella DecodificaAzienda

            public long Id { get; set; }

            public short CodiceAzienda { get; set; }

            public DateTime? UltimaDecorrenzaAmmessa { get; set; }

            public string Tipo { get; set; }

            #endregion DecodificaAziende_CtrlAziendeVOESO
        }
    }
}
