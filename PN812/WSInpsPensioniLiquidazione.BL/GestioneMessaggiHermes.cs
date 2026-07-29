using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneMessaggiHermes
    {
        #region public methods
        /// <summary>
        /// Ritorna l'elenco dei messaggi Hermes
        /// </summary>
        /// <param name="getMessaggiHermesAttivi">Bit di determinazione del flusso logico</param>
        /// <param name="elencoMessaggiHermes">Lista messaggi Hermes</param>
        public static void GetMessaggiHermes(bool getMessaggiHermesAttivi, string tipoApp, out List<MessaggiHermes> elencoMessaggiHermes)
        {
            elencoMessaggiHermes = null;
            List<BLCommon.GestioneMessaggiHermes.DatiMessaggiHermes> elencoMessaggiHermesBL = null;
            if (getMessaggiHermesAttivi)
                BLCommon.GestioneMessaggiHermes.GetMessaggiHermesAttivi(tipoApp, out elencoMessaggiHermesBL);
            else
                BLCommon.GestioneMessaggiHermes.GetAllMessaggiHermes(tipoApp, out elencoMessaggiHermesBL);

            if (elencoMessaggiHermesBL != null)
            {
                elencoMessaggiHermes = new List<MessaggiHermes>();
                foreach (BLCommon.GestioneMessaggiHermes.DatiMessaggiHermes mBL in elencoMessaggiHermesBL)
                {
                    MessaggiHermes mBLComplex = new MessaggiHermes();
                    Utility.ValorizzaOggetti(mBL, mBLComplex);
                    elencoMessaggiHermes.Add(mBLComplex);
                }
            }
        }

        /// <summary>
        /// Salva messaggio Hermes
        /// </summary>
        /// <param name="messaggioHermes">messaggio Hermes</param>
        public static void StoreMessaggioHermes(MessaggiHermes messaggioHermes)
        {
            messaggioHermes.TimeStamp = DateTime.Now;
            BLCommon.GestioneMessaggiHermes.DatiMessaggiHermes messaggioHermesBL = new BLCommon.GestioneMessaggiHermes.DatiMessaggiHermes();
            Utility.ValorizzaOggetti(messaggioHermes, messaggioHermesBL);
            BLCommon.GestioneMessaggiHermes.SalvaMessaggioHermes(messaggioHermesBL);
        }

        /// <summary>
        /// Cancella messaggio Hermes
        /// </summary>
        /// <param name="messaggioHermes">messaggio Hermes</param>
        public static void DeleteMessaggioHermes(MessaggiHermes messaggioHermes)
        {
            BLCommon.GestioneMessaggiHermes.DeleteMessaggioHermes(messaggioHermes.Id);
        }
        #endregion public methods
    }
}
