using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.Entity;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.Mocks
{
    public class MocksGestioneSAI
    {
        public static void GetDatiSAI_Mocks(out SAI datiSAI_Mocks)
        {
            List<GestioneControlliDinamici.ControlloDinamico> elencoControlliDinamici = null;
            GestioneControlliDinamici.GetControlliDinamici(out elencoControlliDinamici);

            Utility.TipoCalcolo tipoCalcolo = Utility.TipoCalcolo.NonValido;

            foreach (GestioneControlliDinamici.ControlloDinamico controlloDinamico in elencoControlliDinamici)
            {
                if (controlloDinamico.NomeControllo.Equals("TipoCalcoloENPALS"))
                {
                    GestionePensione.DatiPensione datiPensione = new GestionePensione.DatiPensione();
                    datiPensione.Gestione = "018";
                    tipoCalcolo = Utility.GetTipoCalcoloById(byte.Parse(controlloDinamico.ValoreControllo), datiPensione, Utility.TipoAppartenenza.AGO);
                }
            }

            datiSAI_Mocks = new SAI();

            datiSAI_Mocks.GETSAI_DT_DECORRENZA = "20100301";

            switch (tipoCalcolo)
            {
                case Utility.TipoCalcolo.Contributivo: //Contributivo
                    datiSAI_Mocks.GETSAI_COEFF_TRASF = 0;
                    datiSAI_Mocks.GETSAI_MONT_CMP = 324.435M;
                    datiSAI_Mocks.GETSAI_IMP_CONTR = 123.45M;
                    break;
                case Utility.TipoCalcolo.Retributivo: //Retributivo
                    datiSAI_Mocks.GETSAI_NR_CTB_ANTE = 1;
                    datiSAI_Mocks.GETSAI_NR_CTB_POST = 1;
                    datiSAI_Mocks.GETSAI_NR_CTB_QUOA = 2;
                    datiSAI_Mocks.GETSAI_NR_CTB_QUOB = 2;
                    datiSAI_Mocks.GETSAI_RTB_MED_540 = 123.45M;
                    datiSAI_Mocks.GETSAI_RTB_MED_POST = 123.45M;
                    datiSAI_Mocks.GETSAI_IMP_QUA = 123.45M;
                    datiSAI_Mocks.GETSAI_IMP_QUB = 123.45M;
                    datiSAI_Mocks.GETSAI_IMP_PRT = 123.45M;
                    datiSAI_Mocks.OBM_CM_IMP_RTV = 123.45M;
                    break;
                case Utility.TipoCalcolo.Misto: //Misto
                    datiSAI_Mocks.GETSAI_COEFF_TRASF = 0;
                    datiSAI_Mocks.GETSAI_MONT_CMP = 324.435M;
                    datiSAI_Mocks.GETSAI_IMP_CONTR = 123.45M;

                    datiSAI_Mocks.GETSAI_NR_CTB_ANTE = 1;
                    datiSAI_Mocks.GETSAI_NR_CTB_POST = 1;
                    datiSAI_Mocks.GETSAI_NR_CTB_QUOA = 2;
                    datiSAI_Mocks.GETSAI_NR_CTB_QUOB = 2;
                    datiSAI_Mocks.GETSAI_RTB_MED_540 = 123.45M;
                    datiSAI_Mocks.GETSAI_RTB_MED_POST = 123.45M;
                    datiSAI_Mocks.GETSAI_IMP_QUA = 123.45M;
                    datiSAI_Mocks.GETSAI_IMP_QUB = 123.45M;
                    datiSAI_Mocks.GETSAI_IMP_PRT = 123.45M;
                    datiSAI_Mocks.OBM_CM_IMP_RTV = 123.45M;
                    break;
            }

            // Liquidazione Pensione
            datiSAI_Mocks.GETSAI_DT_PRI_CTB = "19650101";
            datiSAI_Mocks.GETSAI_AAMM_TRA_DIR = 2211;
            datiSAI_Mocks.GETSAI_RAG_PREV = 'A';
            datiSAI_Mocks.GETSAI_GRU_PREV = 'A';
            datiSAI_Mocks.GETSAI_GRU_DIR = 'A';
            datiSAI_Mocks.GETSAI_NR_TOT_CTB = 200;
            datiSAI_Mocks.GETSAI_NR_TOT_CTB_OBG = 200;
            datiSAI_Mocks.GETSAI_ETA_MAT_DIR = 2211;
            datiSAI_Mocks.GETSAI_ETA_MAT_MIS = 2211;
            datiSAI_Mocks.GETSAI_QUAL_PREV = "AAA";
            datiSAI_Mocks.OBM_CM_DT_FINESTRA = "20100301";
            datiSAI_Mocks.GETSAI_NR_CTB_MIS = 200;
            datiSAI_Mocks.GETSAI_NR_CTB_DIR = 200;
            datiSAI_Mocks.GETSAI_TOT_CTB_QUAL = 200;
            datiSAI_Mocks.GETSAI_TOT_CTB_QUAL_QNQ = 150;
            datiSAI_Mocks.GETSAI_TOT_CTB_QUAL_TRI = 50;
            datiSAI_Mocks.GETSAI_NR_CTB_NL222 = 200;
            datiSAI_Mocks.GETSAI_NR_CTB_NL155 = 200;

            // Maggiorazioni e Benefici
            datiSAI_Mocks.GETSAI_NR_CTB_NVV = 200;
            datiSAI_Mocks.GETSAI_IND_IBT = '0';
        }
    }
}
