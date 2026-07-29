using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration;
using INPS.Pensioni.Liquidazione.BLCommon;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaSbloccoDomanda
    {
        #region public members
        public static bool SbloccoDomandaByDatiPensione(GestionePensione.DatiPensione datiPensione, Utility.TipoAppartenenza? tipoAppRuolo, short sedeOperatore, short centroOperativoOperatore,
            out string sedeDiversa, out string errori)
        {
            errori = string.Empty;
            sedeDiversa = string.Empty;
            try
            {
                if (datiPensione == null)
                {
                    errori = "Domanda non presente nel database. Controllare numero domanda inserito.";
                    return false;
                }

                Utility.TipoAppartenenza? tipoAppDomanda = Utility.GetTipoAppartenenza(datiPensione.IndConvInt.Value, datiPensione.Gestione);
                if (!Utility.IsTipoAppartenenzaEquals(tipoAppDomanda, tipoAppRuolo))
                {
                    errori = "Ruolo Utente non abilitato allo sblocco della domanda";
                    return false;
                }

                if (Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) != sedeOperatore || Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)) != centroOperativoOperatore)
                {
                    errori = "La sede dell'operatore non coincide con la sede della domanda selezionata (" + Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0') + ").";
                    sedeDiversa = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
                    return false;
                }

                if (!GestioneWebDom.SbloccaDomandaWebDom(datiPensione.NDomus, out errori))
                    return false;

                GestioneSbloccoDomanda.DatiSbloccoDomanda datiSbloccoDomanda = new GestioneSbloccoDomanda.DatiSbloccoDomanda();
                datiSbloccoDomanda.NDomus = datiPensione.NDomus;
                GestioneSbloccoDomanda.EliminaSbloccoDomanda(datiSbloccoDomanda);
            }
            catch (Exception Ex)
            {
                errori = "Errore nel metodo SbloccoDomanda: " + Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }
            return true;
        }
        #endregion public members
    }
}
