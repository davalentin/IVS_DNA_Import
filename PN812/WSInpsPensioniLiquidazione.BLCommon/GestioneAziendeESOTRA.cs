using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAziendeESOTRA
    {
        public static void GetDecodificaAziendeESOTRA(out List<DecAziendeESOTRA> elencoAziendeESOTRA)
        {
            elencoAziendeESOTRA = null;
            List<CtrlAziendaESOTRA> elencoDecodificaAziendeESOTRA = null;
            DAGestioneCtrlAziendaESOTRA.GetDecodificaAziendaESOTRA(out elencoDecodificaAziendeESOTRA);
            if (elencoDecodificaAziendeESOTRA != null && elencoDecodificaAziendeESOTRA.Count > 0)
            {
                elencoAziendeESOTRA = new List<DecAziendeESOTRA>();
                foreach (CtrlAziendaESOTRA decodificaAziendaESOTRA in elencoDecodificaAziendeESOTRA)
                {
                    DecAziendeESOTRA AziendaESOTRA = new DecAziendeESOTRA();
                    Utility.ValorizzaOggetti(decodificaAziendaESOTRA, AziendaESOTRA);
                    elencoAziendeESOTRA.Add(AziendaESOTRA);
                }
            }
        }

        public static void GetDecodificaAziendaESOTRAByIdCodiceAzienda(short idAzienda, out DecAziendeESOTRA aziendaESOTRA)
        {
            aziendaESOTRA = null;
            CtrlAziendaESOTRA aziendaESOTRADatabase = null;
            DAGestioneCtrlAziendaESOTRA.GetDecodificaAziendaESOTRAByIdCodiceAzienda(idAzienda, out aziendaESOTRADatabase);
            if (aziendaESOTRADatabase != null)
            {
                aziendaESOTRA = new DecAziendeESOTRA();
                Utility.ValorizzaOggetti(aziendaESOTRADatabase, aziendaESOTRA);
            }
        }

        public static void SalvaAziendeESOTRA(DecAziendeESOTRA decAziendaESOTRA, GestioneDecodificaAzienda.DecAzienda decAzienda) /*2 oggetti, del blcommon della classe innestata qui per aziende ESOTRA e del blcommon per aziende */
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab aziende ESOTRA con valorizzaoggetti
                CtrlAziendaESOTRA decodificaAziendaESOTRA = new CtrlAziendaESOTRA(); /*oggetto del datacommon per tab aziendeESOTRA*/
                Utility.ValorizzaOggetti(decAziendaESOTRA, decodificaAziendaESOTRA);

                //mapping per tab aziende con valorizzaoggetti
                DecodificaAzienda decodificaAzienda = new DecodificaAzienda();
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);

                DAGestioneCtrlAziendaESOTRA.InsertAziendeESOTRA(decodificaAziendaESOTRA, decodificaAzienda);/*salva 2 oggetti del datacommon nell'oggetto del datacommon*/

                transactionScope.Complete();
            }
        }

        public static void DeleteAziendaESOTRA(string codiceAziendaTraduzioneSuGP) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCtrlAziendaESOTRA.DeleteAziendaESOTRA(codiceAziendaTraduzioneSuGP);

                transactionScope.Complete();
            }
        }


        public class DecAziendeESOTRA
        {
            #region DecodificaAziende_CtrlAziendeESOTRA
            //CodiceAzienda a video = traduzione su GP
            //CodiceAzienda tabella CtrlAziendaESOTRA è FK con Id tabella DecodificaAzienda

            public long Id { get; set; }

            public short CodiceAzienda { get; set; }

            public DateTime? UltimaDecorrenzaAmmessa { get; set; }

            #endregion DecodificaAziende_CtrlAziendeESOTRA
        }
    }
}
