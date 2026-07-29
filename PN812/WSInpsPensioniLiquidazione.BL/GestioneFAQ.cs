using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneFAQ
    {
        public static void GetFAQ(string tipoApp, out List<FAQ> listaFAQ)
        {
            listaFAQ = null;
            List<BLCommon.GestioneFAQ.DatiFAQ> listaFAQBL = new List<BLCommon.GestioneFAQ.DatiFAQ>();
            BLCommon.GestioneFAQ.GetFAQs(tipoApp, out listaFAQBL);
            if (listaFAQBL != null && listaFAQBL.Count > 0)
            {
                listaFAQ = new List<FAQ>();
                foreach (BLCommon.GestioneFAQ.DatiFAQ faqBL in listaFAQBL)
                {
                    FAQ faq = new FAQ();
                    Utility.ValorizzaOggetti(faqBL, faq);
                    listaFAQ.Add(faq);
                }
            }
        }

        public static void StoreFAQ(FAQ faq)
        {
            BLCommon.GestioneFAQ.DatiFAQ faqBL = new BLCommon.GestioneFAQ.DatiFAQ();

            List<BLCommon.GestioneDecodifica.TipologiaFAQ> elencoTipologiaFAQ = null;
            BLCommon.GestioneDecodifica.GetTipologiaFAQ(out elencoTipologiaFAQ);
            BLCommon.GestioneDecodifica.TipologiaFAQ tipologiaFAQ = null;
            bool cambiaCodice = false;
            if (elencoTipologiaFAQ != null && elencoTipologiaFAQ.Count > 0)
            {
                if (string.IsNullOrEmpty(faq.Codice))
                {
                    tipologiaFAQ = elencoTipologiaFAQ.Find(x => x.Codice == faq.Tipologia);
                    if (tipologiaFAQ != null)
                    {
                        faq.Codice = tipologiaFAQ.Codice + tipologiaFAQ.Contatore.ToString();
                        cambiaCodice = true;
                    }
                }
            }

            Utility.ValorizzaOggetti(faq, faqBL);
            BLCommon.GestioneFAQ.SalvaFAQ(faqBL, cambiaCodice);
        }

        public static void DeleteFAQ(FAQ faq)
        {
            BLCommon.GestioneFAQ.DeleteFAQ(faq.Id);
        }
    }
}
