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
    public class GestioneAziendeScadenzaAssegnoGGmmAAAA
    {
        public static void GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out List<DecAziendeScadenzaAssegnoGGmmAAAA> elencoAziendeGGmmAAAA)
        {
            elencoAziendeGGmmAAAA = null;
            List<CtrlAziendeScadenzaAssegnoGGmmAAAA> elencoDBAziendeGGmmAAAA = null;
            DAGestioneCtrlAziendeScadenzaAssegnoGGmmAAAA.GetDecodificaAziendeScadenzaAssegnoGGmmAAAA(out elencoDBAziendeGGmmAAAA);
            if (elencoDBAziendeGGmmAAAA != null && elencoDBAziendeGGmmAAAA.Count > 0)
            {
                elencoAziendeGGmmAAAA = new List<DecAziendeScadenzaAssegnoGGmmAAAA>();
                foreach (CtrlAziendeScadenzaAssegnoGGmmAAAA decodificaAziendaGGmmAAAA in elencoDBAziendeGGmmAAAA)
                {
                    DecAziendeScadenzaAssegnoGGmmAAAA AziendaGGmmAAAA = new DecAziendeScadenzaAssegnoGGmmAAAA();
                    Utility.ValorizzaOggetti(decodificaAziendaGGmmAAAA, AziendaGGmmAAAA);
                    elencoAziendeGGmmAAAA.Add(AziendaGGmmAAAA);
                }
            }
        }

        public static void SalvaAziendeScadenzaAssegnoGGmmAAAA(DecAziendeScadenzaAssegnoGGmmAAAA decAziendaScadenzaAssegnoGGmmAAAA, string siglaCategoria) /*oggetto del bl common classe innestata qui*/
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //mapping per tab ggmmaaaa con valorizza oggetti
                DataCommon.CtrlAziendeScadenzaAssegnoGGmmAAAA decodificaAziendaGGmmAAAA = new CtrlAziendeScadenzaAssegnoGGmmAAAA(); /*oggetto del data common, viene valorizzato con quello del bl common*/
                Utility.ValorizzaOggetti(decAziendaScadenzaAssegnoGGmmAAAA, decodificaAziendaGGmmAAAA);

                DAGestioneCtrlAziendeScadenzaAssegnoGGmmAAAA.InsertAziendeScadenzaAssegnoGGmmAAAA(decodificaAziendaGGmmAAAA, siglaCategoria);

                transactionScope.Complete();
            }
        }

        public static void DeleteAziendeScadenzaAssegnoGGmmAAAA(DecAziendeScadenzaAssegnoGGmmAAAA decAziendaScadenzaAssegnoGGmmAAAA, string siglaCategoria)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                CtrlAziendeScadenzaAssegnoGGmmAAAA aziendaScadenzaAssegnoGGmmAAAA = new CtrlAziendeScadenzaAssegnoGGmmAAAA();
                Utility.ValorizzaOggetti(decAziendaScadenzaAssegnoGGmmAAAA, aziendaScadenzaAssegnoGGmmAAAA);
                DAGestioneCtrlAziendeScadenzaAssegnoGGmmAAAA.DeleteAziendeScadenzaAssegnoGGmmAAAA(aziendaScadenzaAssegnoGGmmAAAA, siglaCategoria);
                transactionScope.Complete();
            }
        }

        public class DecAziendeScadenzaAssegnoGGmmAAAA
        {
            public long Id { get; set; }
            public string TraduzioneSuGP { get; set; }
            public byte? ProgressivoRichiesto { get; set; }
            public string SiglaCatPensione { get; set; }
        }
    }
}
