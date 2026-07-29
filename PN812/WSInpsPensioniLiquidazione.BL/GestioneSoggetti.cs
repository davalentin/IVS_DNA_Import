using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneSoggetti
    {
        #region public members
        public static bool GetSoggettoPerCodiceFiscale(Entity.ParametriARCA parametriARCA, string codiceFiscale, string numDomanda, out Entity.Anagrafica anagrafica, out string errori)
        {
            anagrafica = null;
            errori = "";
            try
			{
				// il codice fiscale non è stato trovato sul db locale: lo cerco su ARCA
				#region Area richiesta per ARCA
				GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
				richiestaArca.Applicazione = parametriARCA.Applicazione;
				richiestaArca.Matricola = parametriARCA.Matricola;
				richiestaArca.Provenienza = parametriARCA.Provenienza;
				richiestaArca.Ruolo = parametriARCA.Ruolo;
				richiestaArca.CodiceFiscaleRichiedente = parametriARCA.CodiceFiscaleRichiedente;
				richiestaArca.CodiceFiscale = codiceFiscale;
				#endregion Area richiesta per ARCA
				if (!GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, numDomanda, out anagrafica, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);
            }
            catch (Exception Ex)
            {
                errori = "Errore nel recupero dei dati anagrafici del soggetto: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }

        public static bool AggiornaSoggettoByArca(Entity.ParametriARCA parametriARCA, string codiceFiscale, string numDomanda, out Entity.Anagrafica anagrafica, out string errori)
        {
            anagrafica = null;
            errori = "";
            try
            {
                #region Area richiesta per ARCA
                GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                richiestaArca.Applicazione = parametriARCA.Applicazione;
                richiestaArca.Matricola = parametriARCA.Matricola;
                richiestaArca.Provenienza = parametriARCA.Provenienza;
                richiestaArca.Ruolo = parametriARCA.Ruolo;
                richiestaArca.CodiceFiscaleRichiedente = parametriARCA.CodiceFiscaleRichiedente;
                richiestaArca.CodiceFiscale = codiceFiscale;
                #endregion Area richiesta per ARCA
                if (!GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, numDomanda, out anagrafica, out errori))
                    throw new INPS.DNA.DnaValidationException(errori);
            }
            catch (INPS.DNA.DnaValidationException Ex)
            {
                errori = "Errore nel recupero dei dati anagrafici del soggetto: " + Ex.Message;
                return false;
            }
            catch (Exception Ex)
            {
                errori = "Errore nel recupero dei dati anagrafici del soggetto: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion public members

        #region private members
        #endregion private members
    }
}
