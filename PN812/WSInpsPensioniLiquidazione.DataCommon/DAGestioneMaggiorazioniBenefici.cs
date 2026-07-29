using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using INPS.DNA.Data;
using INPS.DNA.Logging;
using INPS.Pensioni.Liquidazione.DataCommon;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneMaggiorazioniBenefici
    {
        public static void GetMaggiorazioniBeneficiByIdPensione(long idPensione, out MaggiorazioniBenefici maggiorazioniBenefici)
        {
            maggiorazioniBenefici = null;
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                    maggiorazioniBenefici = (from p in db.MaggiorazioniBeneficis where p.IdPensione == idPensione select p).SingleOrDefault<MaggiorazioniBenefici>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }
        
        public static void SalvaMaggiorazioniBenefici(MaggiorazioniBenefici maggiorazionibenefici)
        {
            using (new MethodExecutionTracer())
            {
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.InsertMaggiorazioniBenefici(maggiorazionibenefici.IdPensione,
                                                            maggiorazionibenefici.NSettimaneBeneficio,
                                                            maggiorazionibenefici.TipoSettimaneBeneficio,
                                                            maggiorazionibenefici.NSettimaneIncremento1Percento,
                                                            maggiorazionibenefici.NSettimaneIncremento05Percento,
                                                            maggiorazionibenefici.Attivitausuranti,
                                                            maggiorazionibenefici.CodiceCieco,
                                                            maggiorazionibenefici.DecorrenzaMaggiorazioneArt6,
                                                            maggiorazionibenefici.ImportoArt6,
                                                            maggiorazionibenefici.Aumento780ContributiArt4,
                                                            maggiorazionibenefici.Aumento780ContributiArt1,
                                                            maggiorazionibenefici.ExInpdai,
                                                            maggiorazionibenefici.ImportoAumento780Contributi,
                                                            maggiorazionibenefici.ImportoComplessivoArt4,
                                                            maggiorazionibenefici.AumentoArt5,
                                                            maggiorazionibenefici.ExInpdaiArt4,
                                                            maggiorazionibenefici.ImportoComplessivoArt5,
                                                            maggiorazionibenefici.AumentoArt3,
                                                            maggiorazionibenefici.ExInpdaiArt3,
                                                            maggiorazionibenefici.ImportoComplessivoArt3,
                                                            maggiorazionibenefici.ExInpdaiArt10,
                                                            maggiorazionibenefici.DecorrenzaMaggiorazioneLegge140,
                                                            maggiorazionibenefici.DecorrenzaMaggiorazioneLegge544,
                                                            maggiorazionibenefici.AumentoMensileLegge161289Art2,
                                                            maggiorazionibenefici.AnniRiduzioneBeneficiArt38Legge02,
                                                            maggiorazionibenefici.Aumento7290,
                                                            maggiorazionibenefici.Aumento7290DC,
                                                            maggiorazionibenefici.AumentoMensileLegge5991Comma9,
                                                            maggiorazionibenefici.AumentoMensileLegge5991Comma2,
                                                            maggiorazionibenefici.ImportoBeneficiCombattente,
                                                            maggiorazionibenefici.Sentenza495240,
                                                            maggiorazionibenefici.DecorrenzaVariazione,
                                                            maggiorazionibenefici.Cessazione,
                                                            maggiorazionibenefici.CodiceLeggeGruppo,
                                                            maggiorazionibenefici.CodiceLeggeSottogruppo,
                                                            maggiorazionibenefici.MaggioreAnzianitaConcessa,
                                                            maggiorazionibenefici.MancataContribuzione,
                                                            maggiorazionibenefici.CodiceBenefici,
                                                            maggiorazionibenefici.SettimaneBenefici,
                                                            maggiorazionibenefici.CodiceInvalidita80Percento,
                                                            maggiorazionibenefici.CessazioneMaggiorazioneSociale,
                                                            maggiorazionibenefici.CodiceRequisitiLegge50392Art2,
                                                            maggiorazionibenefici.DecorrenzaInv80,
                                                            maggiorazionibenefici.NSettIncrementoPrepensionamento,
                                                            maggiorazionibenefici.Articolo1Legge5991,
                                                            maggiorazionibenefici.MensileLegge5991,
                                                            maggiorazionibenefici.ExCombattente,
                                                            maggiorazionibenefici.RMSSenzaLegge33670QA,
                                                            maggiorazionibenefici.RMSSenzaLegge33670QB,
                                                            maggiorazionibenefici.PercentualeMaggiorazioneSenzaLegge33670,
                                                            maggiorazionibenefici.DecorrenzaMaggiorazioneSociale,
                                                            maggiorazionibenefici.DirittoScattiLegge336,
                                                            maggiorazionibenefici.SettimaneBeneficioAA,
                                                            maggiorazionibenefici.SettimaneBeneficioMM,
                                                            maggiorazionibenefici.SettimaneBeneficioGG,
                                                            maggiorazionibenefici.IsBeneficioArt24Comma15BisFromFELPE,
                                                            maggiorazionibenefici.MaggiorazioneAmianto,
                                                            maggiorazionibenefici.MaggiorazioneInv74, 
                                                            maggiorazionibenefici.IsBeneficioApePrecociFromFELPE,
                                                            maggiorazionibenefici.SettAnzContribPost311295,
                                                            maggiorazionibenefici.DataNonVedenteDal,
                                                            maggiorazionibenefici.PercentualeMaggiorazione,
                                                            maggiorazionibenefici.NSettIntegrazioneContributivaConcessa,
                                                            maggiorazionibenefici.ImportoComplessivoArt1,
                                                            maggiorazionibenefici.Articolo6140);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure InsertMaggiorazioniBenefici");
                }
                db.Connection.Close();
            }
        }

        public static void EliminaMaggiorazioniBeneficiByIdPensione(long idPensione)
        {
            using (new MethodExecutionTracer())
            {                          
                PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                int result = db.DeleteMaggiorazioniBenefici(idPensione);
                if (result != 0)
                {
                    throw new INPS.DNA.DnaApplicationException("Si è verificato un errore durante l'esecuzione della Stored Procedure MaggiorazioniBenefici");
                }
                db.Connection.Close();
            }
        }
    }
}
