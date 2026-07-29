using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneAziendeESOTEL
    {
        public static void GetDecodificaAziendeESOTEL(out List<DecAziendeESOTEL> elencoAziendeESOTEL)
        {
            elencoAziendeESOTEL = null;
            List<CtrlAziendaESOTRA> elencoDecodificaAziendeESOTEL = null;
            DAGestioneCtrlAziendaESOTEL.GetDecodificaAziendaESOTRA(out elencoDecodificaAziendeESOTEL);
            if (elencoDecodificaAziendeESOTEL != null && elencoDecodificaAziendeESOTEL.Count > 0)
            {
                elencoAziendeESOTEL = new List<DecAziendeESOTEL>();
                foreach (CtrlAziendaESOTRA decodificaAziendaESOTEL in elencoDecodificaAziendeESOTEL)
                {
                    DecAziendeESOTEL AziendaESOTEL = new DecAziendeESOTEL();
                    Utility.ValorizzaOggetti(decodificaAziendaESOTEL, AziendaESOTEL);
                    elencoAziendeESOTEL.Add(AziendaESOTEL);
                }
            }
        }

        public static void GetDecodificaAziendaESOTELByIdCodiceAzienda(short idAzienda, out DecAziendeESOTEL aziendaESOTEL)
        {
            aziendaESOTEL = null;
            CtrlAziendaESOTRA aziendaESOTELDatabase = null;
            DAGestioneCtrlAziendaESOTEL.GetDecodificaAziendaESOTRAByIdCodiceAzienda(idAzienda, out aziendaESOTELDatabase);
            if (aziendaESOTELDatabase != null)
            {
                aziendaESOTEL = new DecAziendeESOTEL();
                Utility.ValorizzaOggetti(aziendaESOTELDatabase, aziendaESOTEL);
            }
        }

        public static void SalvaAziendeESOTEL(DecAziendeESOTEL decAziendaESOTEL, GestioneDecodificaAzienda.DecAzienda decAzienda) /*2 oggetti, del blcommon della classe innestata qui per aziende ESOTEL e del blcommon per aziende */
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab aziende ESOTEL con valorizzaoggetti
                CtrlAziendaESOTRA decodificaAziendaESOTEL = new CtrlAziendaESOTRA(); /*oggetto del datacommon per tab aziendeESOTEL*/
                Utility.ValorizzaOggetti(decAziendaESOTEL, decodificaAziendaESOTEL);

                //mapping per tab aziende con valorizzaoggetti
                DecodificaAzienda decodificaAzienda = new DecodificaAzienda();
                Utility.ValorizzaOggetti(decAzienda, decodificaAzienda);

                DAGestioneCtrlAziendaESOTEL.InsertAziendeESOTRA(decodificaAziendaESOTEL, decodificaAzienda);/*salva 2 oggetti del datacommon nell'oggetto del datacommon*/

                transactionScope.Complete();
            }
        }

        public static void DeleteAziendaESOTEL(string codiceAziendaTraduzioneSuGP) /*oggetto del blcommon*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneCtrlAziendaESOTEL.DeleteAziendaESOTRA(codiceAziendaTraduzioneSuGP);

                transactionScope.Complete();
            }
        }


        public class DecAziendeESOTEL
        {
            #region DecodificaAziende_CtrlAziendeESOTEL
            //CodiceAzienda a video = traduzione su GP
            //CodiceAzienda tabella CtrlAziendaESOTEL è FK con Id tabella DecodificaAzienda

            public long Id { get; set; }

            public short CodiceAzienda { get; set; }

            public DateTime? UltimaDecorrenzaAmmessa { get; set; }

            #endregion DecodificaAziende_CtrlAziendeESOTEL
        }
    }
}
