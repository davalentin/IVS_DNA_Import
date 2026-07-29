using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione.BLCommon
{

    public static class GestioneContribuzioneEnpals
    {
        public static void GetDatiContribuzioneEnpalsByIdPensioneAndTipologia(long idPensione, TipologiaContribuzioneEnpals tipologia, out Entity.DatiContribuzioneEnpals contribuzioneEnpals)
        {
            contribuzioneEnpals = null;
            List<INPS.Pensioni.Liquidazione.DataCommon.ContribuzioneEnpals> lstContribuzioneEnpals;
            DAGestioneContribuzioneEnpals.GetDatiContribuzioneEnpals(idPensione, tipologia.ToString(), out lstContribuzioneEnpals);
            if (lstContribuzioneEnpals != null && lstContribuzioneEnpals.Count > 0)
            {
                contribuzioneEnpals = new Entity.DatiContribuzioneEnpals();
                contribuzioneEnpals.IdPensione = idPensione;
                contribuzioneEnpals.Tipologia = tipologia;
                foreach (var elem in lstContribuzioneEnpals)
                {

                    switch (elem.Quota)
                    {
                        case 'A':
                            contribuzioneEnpals.QuotaA = new Entity.DatiContribuzioneEnpals.Quota();
                            Utility.ValorizzaOggetti(elem, contribuzioneEnpals.QuotaA);
                            break;
                        case 'B':
                            contribuzioneEnpals.QuotaB = new Entity.DatiContribuzioneEnpals.Quota();
                            Utility.ValorizzaOggetti(elem, contribuzioneEnpals.QuotaB);
                            break;
                        case 'C':
                            contribuzioneEnpals.QuotaC = new Entity.DatiContribuzioneEnpals.Quota();
                            Utility.ValorizzaOggetti(elem, contribuzioneEnpals.QuotaC);
                            break;
                    }
                }
            }
        }

        public static void SalvaDatiContributizioneEnpals(GestionePensione.DatiPensione datiPensione, Entity.DatiContribuzioneEnpals contribuzioneEnpals)
        {
            List<INPS.Pensioni.Liquidazione.DataCommon.ContribuzioneEnpals> lstContribuzioneEnpalsDb = new List<ContribuzioneEnpals>();

            if (contribuzioneEnpals != null)
            {
                if (contribuzioneEnpals.QuotaA != null)
                {
                    ContribuzioneEnpals quotaA = new ContribuzioneEnpals();
                    Utility.ValorizzaOggetti(contribuzioneEnpals.QuotaA, quotaA);
                    quotaA.Quota = 'A';
                    quotaA.Tipologia = contribuzioneEnpals.Tipologia.ToString();
                    quotaA.IdPensione = datiPensione.Id;
                    lstContribuzioneEnpalsDb.Add(quotaA);
                }

                if (contribuzioneEnpals.QuotaB != null)
                {
                    ContribuzioneEnpals quotaB = new ContribuzioneEnpals();
                    Utility.ValorizzaOggetti(contribuzioneEnpals.QuotaB, quotaB);
                    quotaB.Quota = 'B';
                    quotaB.Tipologia = contribuzioneEnpals.Tipologia.ToString();
                    quotaB.IdPensione = datiPensione.Id;
                    lstContribuzioneEnpalsDb.Add(quotaB);
                }

                if (contribuzioneEnpals.QuotaC != null)
                {
                    ContribuzioneEnpals quotaC = new ContribuzioneEnpals();
                    Utility.ValorizzaOggetti(contribuzioneEnpals.QuotaC, quotaC);
                    quotaC.Quota = 'C';
                    quotaC.Tipologia = contribuzioneEnpals.Tipologia.ToString();
                    quotaC.IdPensione = datiPensione.Id;
                    lstContribuzioneEnpalsDb.Add(quotaC);

                }
            }
            if (lstContribuzioneEnpalsDb.Count > 0)
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    foreach (var elem in lstContribuzioneEnpalsDb)
                        DAGestioneContribuzioneEnpals.SalvaDatiContribuzioneEnpals(elem);
                    transactionScope.Complete();
                }
            }
        }

        public static void SalvaEntityDatiContributizioneEnpals(GestionePensione.DatiPensione datiPensione, Entity.DatiContribuzioneEnpals contribuzioneEnpals)
        {
            GestioneQuadri.DatiQuadroLiquidazionePensione datiQuadroLiqPens = null;
            GestioneQuadri.DatiQuadroSupplementi datiQuadroSupplementi = null;
            if (contribuzioneEnpals.Tipologia == TipologiaContribuzioneEnpals.SAI)
            {
                GestioneQuadri.GetQuadroLiquidazionePensioneByDatiPensione(datiPensione, out datiQuadroLiqPens);
            }
            else
            {
                GestioneQuadri.GetQuadroSupplementiByDatiPensione(datiPensione, out datiQuadroSupplementi);
            }

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                //nota : il salvataggio per ora è inutie perchè il tab e bloccato.
                //se qualche campo diventa editabile decommentare questa riga
                //SalvaDatiContributizioneEnpals(datiPensione, contribuzioneEnpals);

                if (contribuzioneEnpals.Tipologia == TipologiaContribuzioneEnpals.SAI)
                {
                    if (datiQuadroLiqPens != null)
                        datiQuadroLiqPens.TabContribuzioneEnpals = 2;
                    GestioneQuadri.SalvaQuadroLiquidazionePensione(datiPensione.Id, datiQuadroLiqPens);
                }
                else
                {
                    if (datiQuadroSupplementi != null)
                        datiQuadroSupplementi.TabContribuzioneEnpals = 2;
                    GestioneQuadri.SalvaQuadroSupplementi(datiPensione.Id, datiQuadroSupplementi);
                }
                transactionScope.Complete();
            }
        }
           
        public static void DeleteDatiContribuzioneByIdPensioneAndTipologia(long idPensione, TipologiaContribuzioneEnpals tipologia)
        {
            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                DAGestioneContribuzioneEnpals.CancellaDatiContribuzioneEnpalsByIdPensione(idPensione, tipologia.ToString());
                
                transactionScope.Complete();
            }
        }
    }

    #region Enum
    public enum TipologiaContribuzioneEnpals { SAI, SAS }
    #endregion Enum
}
