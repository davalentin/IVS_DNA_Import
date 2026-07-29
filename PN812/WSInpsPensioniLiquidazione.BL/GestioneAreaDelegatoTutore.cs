using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA;

namespace INPS.Pensioni.Liquidazione
{
	public class GestioneAreaDelegatoTutore
	{
        public static void GetDelegatoByIdPensione(long idPensione, out Entity.Anagrafica anagrafica)
		{
			anagrafica = new INPS.Pensioni.Liquidazione.Entity.Anagrafica();
			try
			{
				GestioneAnagrafica.DatiAnagrafici datiAnagraficaDb = null;
				GestioneDelegatoTutore.GetDelegatoByIdPensione(idPensione, out datiAnagraficaDb);
				if (datiAnagraficaDb != null)
					Utility.ValorizzaOggetti(datiAnagraficaDb, anagrafica);
			}
			catch (DnaExceptionBase)
			{
				throw;			
			}
			catch (Exception Ex)
			{
				throw new DnaApplicationException("Errore nel metodo GetDelegatoByNumeroDomanda.", Ex);
			}
		}

        public static void GetTutoreByIdPensione(long idPensione, out Entity.Anagrafica anagrafica)
		{
			anagrafica = new INPS.Pensioni.Liquidazione.Entity.Anagrafica();
			try
			{
				GestioneAnagrafica.DatiAnagrafici datiAnagraficaDb = null;
				GestioneDelegatoTutore.GetTutoreByIdPensione(idPensione, out datiAnagraficaDb);
				if (datiAnagraficaDb != null)
					Utility.ValorizzaOggetti(datiAnagraficaDb, anagrafica);
			}
			catch (DnaExceptionBase)
			{
				throw;
			}
			catch (Exception Ex)
			{
				throw new DnaApplicationException("Errore nel metodo GetTutoreByNumeroDomanda.", Ex);
			}
		}

		public static void SalvaDelegatoByDatiPensione(GestionePensione.DatiPensione datiPensione, Entity.Anagrafica anagrafica)
		{
			GestioneAnagrafica.DatiAnagrafici datiAnagrafici = new GestioneAnagrafica.DatiAnagrafici();
			if (anagrafica != null)
			{
				Utility.ValorizzaOggetti(anagrafica, datiAnagrafici);
			}
			GestioneDelegatoTutore.SalvaDelegatoByDatiPensione(datiPensione, datiAnagrafici);
		}

		public static void SalvaTutoreByDatiPensione(GestionePensione.DatiPensione datiPensione, Entity.Anagrafica anagrafica)
		{
			GestioneAnagrafica.DatiAnagrafici datiAnagrafici = new GestioneAnagrafica.DatiAnagrafici();
			if (anagrafica != null)
			{
				Utility.ValorizzaOggetti(anagrafica, datiAnagrafici);
			}
			GestioneDelegatoTutore.SalvaTutoreByDatiPensione(datiPensione, datiAnagrafici);
		}

        public static bool ControlsDelegatoTutoreByDatiPensione(GestionePensione.DatiPensione datiPensione, Entity.Anagrafica delegato, Entity.Anagrafica tutore, string codFiscTitolare, bool isRiapertura, 
            out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (delegato != null && delegato.DataMorte.HasValue)
            {
                messaggioVideo = "Non è possibile inserire un Delegato deceduto";
                return false;
            }

            if (tutore != null && tutore.DataMorte.HasValue)
            {
                messaggioVideo = "Non è possibile inserire un Tutore deceduto";
                return false;
            }
            
            if (!GestioneCrossControls.ALL_VerificaDelegheTuteleByIdPensione(datiPensione, delegato != null ? delegato.CodiceFiscale : string.Empty,
                tutore != null ? tutore.CodiceFiscale : string.Empty, 
                tutore != null ? tutore.CodiceTutore : (char?)null, tutore != null ? tutore.CessValAmmSost : (DateTime?)null, codFiscTitolare, isRiapertura, out messaggioVideo))
                return false;
            return true;
        }

        public static void EliminaDelegatoByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneDelegatoTutore.EliminaDelegatoByDatiPensione(datiPensione);
        }

        public static void EliminaTutoreByDatiPensione(GestionePensione.DatiPensione datiPensione)
        {
            GestioneDelegatoTutore.EliminaTutoreByDatiPensione(datiPensione);
        }

	}
}
