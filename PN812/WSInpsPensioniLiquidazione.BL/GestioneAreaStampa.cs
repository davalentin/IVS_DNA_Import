using System;
using System.IO;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaStampa
    {
        #region public members
        public static bool GetStampaByDatiPensione(GestionePensione.DatiPensione datiPensione, out MemoryStream msPDF, out string errore)
        {
            errore = string.Empty;
            msPDF = null;
            try
            {
                if (datiPensione == null)
                    return true;

                GestioneStampa.DatiStampa areaStampa = null;

                //if (datiPensione != null && datiPensione.IsNuovoCalcolo.GetValueOrDefault())
                //    CancelStampaByIdPensione(datiPensione.Id, out errore);

                GestioneStampa.GetStampaByIdPensione(datiPensione.Id, out areaStampa);
                if (areaStampa != null)
                {
                    msPDF = new MemoryStream(areaStampa.PDF.ToArray());
                    return true;
                }
                //recupero stampa da WS StampeWeb
                byte[] PDF = null;
                if (!GestioneStampeWeb.GetStampaDomanda(datiPensione, out PDF, out errore))
                {
                    Logger.WriteError(errore);
                    return true;
                }

                if (PDF == null)
                    return true;

                areaStampa = new GestioneStampa.DatiStampa();
                areaStampa.PDF = (System.Data.Linq.Binary)PDF;

                if (!StoreStampaByIdPensione(datiPensione.Id, areaStampa, out errore))
                {
                    Logger.WriteError(errore);
                    return true;
                }

                msPDF = new MemoryStream(areaStampa.PDF.ToArray());
            }
            catch (Exception Ex)
            {
                errore = "Errore tecnico nel recupero della stampa della domanda";
                string messaggio = string.Format("Errore nel metodo GetStampaByDomanda: {0}", Utility.GetMessageFromException(Ex));
                GestioneLogGenerico.SalvaLogGenerico(datiPensione.NDomus, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, messaggio, null, Ex.StackTrace);
                Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool GetStampaByChiavePensione(string siglaCategoria, string codiceSede, string certificato, out MemoryStream msPDF, out string errore)
        {
            //Log per capire quando questo e se questo metodo è utilizzato o è possibile l'eliminazione
            GestioneLogGenerico.SalvaLogGenerico(0, MethodBase.GetCurrentMethod().Name, Utility.TipoLogGenerico.ErroreApplicativo, string.Format("Metodo GetStampaByChiavePensione chiamato. Chiave Pensione => SiglaCategoria: {0}, CodiceSede: {1}, Certificato: {2}", siglaCategoria, codiceSede, certificato), null, string.Empty);

            errore = string.Empty;
            msPDF = null;
            try
            {
                //recupero stampa da WS StampeWeb
                byte[] PDF = null;
                if (!GestioneStampeWeb.GetStampaDomanda(siglaCategoria, codiceSede, certificato, out PDF, out errore))
                {
                    Logger.WriteError(errore);
                    return true;
                }

                if (PDF == null)
                    return true;

                msPDF = new MemoryStream(PDF);
            }
            catch (Exception Ex)
            {
                errore = "Errore nel metodo GetStampaByChiavePensione: " + Ex.Message;
                Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool StoreStampaByIdPensione(Int64 idPensione, GestioneStampa.DatiStampa areaStampa, out string errore)
        {
            errore = "";
            try
            {
                GestioneStampa.SalvaStampa(idPensione, areaStampa);
            }
            catch (Exception Ex)
            {
                errore = "Errore nel metodo StoreStampaByDomanda: " + Ex.Message;
                Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool CancelStampaByIdPensione(Int64 idPensione, out string errore)
        {
            errore = "";
            try
            {
                GestioneStampa.EliminaStampaByIdPensione(idPensione);
            }
            catch (Exception Ex)
            {
                errore = "Errore nel metodo CancelStampaByDomanda: " + Ex.Message;
                Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion public members
    }
}
