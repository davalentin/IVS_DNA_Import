using INPS.Pensioni.Liquidazione.BLCommon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaNuovoCalcolo
    {

        public static bool InsertOrUpdateNuovoCalcolo(GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo datiNuovoCalcolo, out string errori)
        {
            errori = string.Empty;
            Guid guid = Guid.NewGuid();
            try
            {
                GestioneLogSoap.SalvaLogSoap(datiNuovoCalcolo, Utility.Servizio.SrvNuovoCalcolo, Utility.MetodoServizio.InsertOrUpdateNuovoCalcolo, Utility.SOAPLogDirection.IN, datiNuovoCalcolo.NDomus != null ? datiNuovoCalcolo.NDomus.ToString() : null, guid);
                GestioneNuovoCalcolo.InsertOrUpdateNuovoCalcolo(datiNuovoCalcolo);
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(datiNuovoCalcolo.NDomus != null ? (long)datiNuovoCalcolo.NDomus : 0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, Ex != null ? Ex.Message : null, null, Ex != null ? Ex.StackTrace : null);
                return false;
            }

            return true;
        }

        public static bool GetEsitoNuovoCalcolo(long? Ndomus, string TransactionId, out GestioneNuovoCalcolo.DatiEsitoNuovoCalcolo datiNuovoCalcolo, out GestioneNuovoCalcolo.RispostaJson rispostaNuovoCalcolo, out string errori)
        {
            errori = string.Empty;
            datiNuovoCalcolo = null;
            rispostaNuovoCalcolo = null;
            try
            {
                if (!string.IsNullOrEmpty(TransactionId))
                    GestioneNuovoCalcolo.GetRispostaNuovoCalcoloByTransactionId(TransactionId, out datiNuovoCalcolo);
                else if (Ndomus != null)
                    GestioneNuovoCalcolo.GetRispostaNuovoCalcoloByNDomus(Ndomus.GetValueOrDefault(), out datiNuovoCalcolo);

                if (datiNuovoCalcolo == null || string.IsNullOrEmpty(datiNuovoCalcolo.Risposta))
                {
                    errori = "L'operazione di calcolo è in corso. Non appena disponibile l'esito, sarà visibile accedendo nuovamente alla posizione. ";
                    return false;
                }
                else
                {
                    rispostaNuovoCalcolo = Newtonsoft.Json.JsonConvert.DeserializeObject<GestioneNuovoCalcolo.RispostaJson>(datiNuovoCalcolo.Risposta);

                    if (rispostaNuovoCalcolo != null && rispostaNuovoCalcolo.errors != null && rispostaNuovoCalcolo.errors.Length > 0)
                    {
                        errori = (!string.IsNullOrEmpty(rispostaNuovoCalcolo.errors.FirstOrDefault().code) ? rispostaNuovoCalcolo.errors.FirstOrDefault().code : "") + " " + (!string.IsNullOrEmpty(rispostaNuovoCalcolo.errors.FirstOrDefault().message) ? rispostaNuovoCalcolo.errors.FirstOrDefault().message : "" );
                    }
                    return true;
                }
            }
            catch (Exception Ex)
            {
                errori = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                GestioneLogGenerico.SalvaLogGenerico(Ndomus != null ? (long)Ndomus : 0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, (TransactionId != null ? TransactionId : "") + "-" + (Ex != null ? Ex.Message : ""), null, Ex != null ? Ex.StackTrace : null);
            }
            return false;
        }

    }
}
