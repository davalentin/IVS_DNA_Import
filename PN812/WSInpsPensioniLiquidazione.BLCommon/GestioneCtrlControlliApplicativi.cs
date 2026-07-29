using INPS.Pensioni.Liquidazione.DataCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneCtrlControlliApplicativi
    {
        public static void GetControlloApplicativo(Enum enumControllo, out DatiCtrlControlliApplicativi datiCtrlControlliApplicativi)
        {
            string tipoApp = null;
            if (enumControllo.GetType() == typeof(EnumNomeControllo.AGO))
                tipoApp = "AGO";
            else if (enumControllo.GetType() == typeof(EnumNomeControllo.FS))
                tipoApp = "FS";
            else if (enumControllo.GetType() == typeof(EnumNomeControllo.CI))
                tipoApp = "CI";

            datiCtrlControlliApplicativi = null;
            CtrlControlliApplicativi controlloApplicativo = null;
            DAGestioneCtrlControlliApplicativi.GetControlloApplicativo(enumControllo.ToString(), tipoApp, out controlloApplicativo);
            if (controlloApplicativo != null)
            {
                datiCtrlControlliApplicativi = new DatiCtrlControlliApplicativi();
                Utility.ValorizzaOggetti(controlloApplicativo, datiCtrlControlliApplicativi);
            }
        }

        public static bool CheckControlloApplicativoAttivoByData(Enum enumControllo, DateTime dataSistema)
        {
            string tipoApp = null;
            if (enumControllo.GetType() == typeof(EnumNomeControllo.AGO))
                tipoApp = "AGO";
            else if (enumControllo.GetType() == typeof(EnumNomeControllo.FS))
                tipoApp = "FS";
            else if (enumControllo.GetType() == typeof(EnumNomeControllo.CI))
                tipoApp = "CI";

            CtrlControlliApplicativi controlloApplicativo = null;
            DAGestioneCtrlControlliApplicativi.GetControlloApplicativo(enumControllo.ToString(), tipoApp, out controlloApplicativo);
            if (controlloApplicativo != null)
            {
                if ((!controlloApplicativo.DataDal.HasValue || Utility.DataSuccessivaA(dataSistema, controlloApplicativo.DataDal.Value)) &&
                    (!controlloApplicativo.DataAl.HasValue || !Utility.DataStrettamenteSuccessivaA(dataSistema, controlloApplicativo.DataAl.Value)))
                    return true;
            }

            return false;
        }

        #region DatiCtrlControlliApplicativi
        public class DatiCtrlControlliApplicativi
        {
            public string Nome { get; set; }
            public string TipoApp { get; set; }
            public string IdentificativoAggiuntivo { get; set; }
            public DateTime? DataDal { get; set; }
            public DateTime? DataAl { get; set; }
        }
        #endregion DatiCtrlControlliApplicativi

        #region Enum Nome Controllo
        public class EnumNomeControllo
        {
            public enum AGO
            {
                /// <summary>
                /// Se la domanda è un cumulo esterno blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_CUMULO_ESTERNO,
                /// <summary>
                /// Se la domanda ha titolare residente all'estero o modalità di pagamento estera blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_ESTERO,
                /// <summary>
                /// Se la domanda è un cumulo, effettuo lo scarico delle trattenute relative alle quote pensione
                /// </summary>
                SCARICO_TRATTENUTE_CUMULO,
                /// <summary>
                /// Se la domanda è un'anticipata per legge di bilancio 2019 blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_ANTICIPATA2019,
                /// <summary>
                /// Se attivo, bypassa i controlli sui requisiti di età
                /// </summary>
                BYPASS_REQUISITI_ETA,
                /// <summary>
                /// Se la domanda è una ricostituzione in cumulo, blocco il recupero delle informazioni da IVS
                /// </summary>
                BLOCCO_RIC_CUMULO_AUTOMATICHE,
                /// <summary>
                /// Se la domanda è una ricostituzione in cumulo VOCUM, blocco se manuale
                /// </summary>
                BLOCCO_RIC_VOCUM,
                /// <summary>
                /// Se la domanda è una trasformata in cumulo, blocco il recupero delle informazioni da IVS
                /// </summary>
                BLOCCO_TRF_CUMULO_AUTOMATICHE,
                /// <summary>
                /// Se la domanda è una totalizzazione cassa esterna blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_TOTALIZZAZIONE_ESTERNO
            }

            public enum CI
            {
                /// <summary>
                /// Se la domanda ha titolare residente all'estero o modalità di pagamento estera blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_ESTERO,
                /// <summary>
                /// Se la domanda è un'anticipata per legge di bilancio 2019 blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_ANTICIPATA2019,
                /// <summary>
                /// Se attivo, bypassa i controlli sui requisiti di età
                /// </summary>
                BYPASS_REQUISITI_ETA
            }

            public enum FS
            {
                /// <summary>
                /// Se la domanda ha titolare residente all'estero o modalità di pagamento estera blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_ESTERO,
                /// <summary>
                /// Se la domanda è un'anticipata per legge di bilancio 2019 blocco la funzionalità di invio al calcolo
                /// </summary>
                BLOCCOCALCOLO_ANTICIPATA2019,
                /// <summary>
                /// Se attivo, utilizza il nuovo tracciato di calcolo per i fondi FS e PT
                /// </summary>
                UTILIZZANUOVOTRACCIATO_FSPT,
                /// <summary>
                /// Se attivo, bypassa i controlli sui requisiti di età
                /// </summary>
                BYPASS_REQUISITI_ETA
            }
        }
        #endregion Enum Nome Controllo
    }
}
