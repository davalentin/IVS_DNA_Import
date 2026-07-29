using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaRiassegnazioneDomanda
    {
        public static void RicercaDomanda(ref DatiRiassegnazioneDomanda datiRiassegnazioneDomanda, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (!ControlRicercaDomanda(datiRiassegnazioneDomanda, datiPensione, out messaggioVideo))
            {
                if (string.IsNullOrEmpty(datiRiassegnazioneDomanda.SedeDiversa))
                    return;
            }

            //recupero la descrizione dello stato pensione
            Utility.StatoPensione? stato = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value);
            string statoPensione = Utility.GetDescription(stato);

            datiRiassegnazioneDomanda.NumeroDomanda = datiPensione.NDomus;
            datiRiassegnazioneDomanda.StatoPensione = statoPensione;
            datiRiassegnazioneDomanda.VecchiaMatricola = datiPensione.MatricolaUtenteAcquisizione;
        }

        public static void AggiornaDomanda(string nuovaMatricola, GestionePensione.DatiPensione datiPensione, out DatiRiassegnazioneDomanda datiOutputRiassegnazioneDomanda)
        {
            datiOutputRiassegnazioneDomanda = null;

            if (datiPensione != null)
            {
                datiPensione.MatricolaUtenteAcquisizione = nuovaMatricola;

                GestionePensione.SalvaPensione(datiPensione);

                //recupero la descrizione dello stato pensione
                Utility.StatoPensione? stato = Utility.GetStatoPensioneByCodice(datiPensione.StatoPensione.Value);
                string statoPensione = Utility.GetDescription(stato);

                datiOutputRiassegnazioneDomanda = new DatiRiassegnazioneDomanda();
                datiOutputRiassegnazioneDomanda.NumeroDomanda = datiPensione.NDomus;
                datiOutputRiassegnazioneDomanda.StatoPensione = statoPensione;
                datiOutputRiassegnazioneDomanda.VecchiaMatricola = datiPensione.MatricolaUtenteAcquisizione;
            }
        }

        private static bool ControlRicercaDomanda(DatiRiassegnazioneDomanda datiInputRiassegnazioneDomanda, GestionePensione.DatiPensione datiPensione, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;


            if (datiPensione == null)
            {
                messaggioVideo = "Numero domanda non presente nel database";
                return false;
            }

            if (datiInputRiassegnazioneDomanda.NumeroDomanda == 0)
            {
                messaggioVideo = "Il Numero di Domanda non è valorizzato";
                return false;
            }

            if (datiInputRiassegnazioneDomanda.NumeroDomanda.ToString().StartsWith("0") || datiInputRiassegnazioneDomanda.NumeroDomanda.ToString().Length != 13)
            {
                messaggioVideo = "Il Numero di Domanda non può avere come prima cifra 0 e deve essere lungo 13";
                return false;
            }

            Utility.TipoAppartenenza? tipoApp = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);

            if (tipoApp.HasValue && datiInputRiassegnazioneDomanda.TipoAppOperatore.HasValue && datiInputRiassegnazioneDomanda.TipoAppOperatore.Value != tipoApp)
            {
                messaggioVideo = "Il tipo appartenenza dell'operatore non coincide con il tipo appartenenza della domanda ricercata";
                return false;
            }


            if (datiPensione.TipoAutomazione == null && (datiInputRiassegnazioneDomanda.Ruolo.HasValue &&
                datiInputRiassegnazioneDomanda.Ruolo.Value != Utility.Ruolo.AMMINISTRATORE &&
                datiInputRiassegnazioneDomanda.Ruolo.Value != Utility.Ruolo.DIRETTORE_RDP))
            {
                messaggioVideo = "La riassegnazione della domanda può essere effettuata solamente da un Amministratore o da un Direttore_RdP";
                return false;
            }

            if (!string.IsNullOrEmpty(datiInputRiassegnazioneDomanda.StatoPensione) && datiInputRiassegnazioneDomanda.StatoPensione.Equals("CALCOLATA"))
            {
                messaggioVideo = "Non è possibile riassegnare una domanda se il suo stato è 'CALCOLATA'";
                return false;
            }

            string sSede = Utility.GetCodiceSedeLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(4, '0') + Utility.GetCentroOperativoLavorazione(datiPensione, Utility.IsRiaperturaDomanda(datiPensione.Id)).ToString().PadLeft(2, '0');
            int sede = int.Parse(sSede);
            if (datiInputRiassegnazioneDomanda.Sede != sede)
            {
                messaggioVideo = "La sede dell'operatore non coincide con la sede della domanda selezionata (" + sede.ToString().PadLeft(6, '0') + ")";
                //cambio sede domanda deve essere abilito solo per il l'amministratore
                if (datiInputRiassegnazioneDomanda.Ruolo.HasValue && datiInputRiassegnazioneDomanda.Ruolo.Value == Utility.Ruolo.AMMINISTRATORE)
                {
                    datiInputRiassegnazioneDomanda.SedeDiversa = sSede;

                }
                return false;
            }

            //Controlli richiamati solamente se si è in fase di aggiornamento matricola
            if (datiInputRiassegnazioneDomanda.TipoOperazione.HasValue && datiInputRiassegnazioneDomanda.TipoOperazione.Value == Utility.TipoOperazione.UPDATE)
            {
                if (string.IsNullOrEmpty(datiInputRiassegnazioneDomanda.NuovaMatricola))
                {
                    messaggioVideo = "Inserire la nuova matricola da riassegnare alla domanda";
                    return false;
                }

                if (datiInputRiassegnazioneDomanda.NuovaMatricola.Length != 8)
                {
                    messaggioVideo = "La matricola deve essere lunga 8 caratteri";
                    return false;
                }

                long matricola = 0;
                long.TryParse(datiInputRiassegnazioneDomanda.NuovaMatricola, out matricola);
                if (!(datiInputRiassegnazioneDomanda.NuovaMatricola.ToUpperInvariant().StartsWith("E") || matricola != 0))
                {
                    messaggioVideo = "La matricola deve iniziare con la lettera 'E' oppure deve contenere solo numeri";
                    return false;
                }

                if (datiInputRiassegnazioneDomanda.NuovaMatricola.ToUpperInvariant().StartsWith("E"))
                {
                    long.TryParse(datiInputRiassegnazioneDomanda.NuovaMatricola.Substring(1, datiInputRiassegnazioneDomanda.NuovaMatricola.Length - 1), out matricola);
                    if (matricola == 0)
                    {
                        messaggioVideo = "La matricola che inizia con la lettera 'E', successivamente deve contenere solo numeri";
                        return false;
                    }
                }

                //Controllo se, tra la ricerca domanda e l'update, un altro amministratore/direttore_RdP ha già riassegnato la domanda ad un altra matricola
                if (!string.IsNullOrEmpty(datiInputRiassegnazioneDomanda.VecchiaMatricola) && !datiInputRiassegnazioneDomanda.VecchiaMatricola.Equals(datiPensione.MatricolaUtenteAcquisizione))
                {
                    messaggioVideo = "La Matricola Operatore non corrisponde con la precedente. La domanda è stata riassegnata ad un altro operatore. Rieseguire la ricerca";
                    return false;
                }

                //SCRIWO
                GestioneWSSCRIWO.AggiornaStatoLavorazione(datiPensione, null, matricola.ToString(), datiPensione.CodiceSede, true);
            }



            return true;
        }

        #region nested class
        public class DatiRiassegnazioneDomanda
        {
            #region Private Properties

            #region Input/Output Parameters
            private long _NumeroDomanda;
            private string _StatoPensione;
            private string _VecchiaMatricola;
            #endregion Input/Output Parameters

            #region Input Parameters
            private string _NuovaMatricola;
            private Utility.TipoOperazione? _TipoOperazione;
            private Utility.TipoAppartenenza? _TipoAppOperatore;
            private Utility.Ruolo? _Ruolo;
            private int? _Sede;
            #endregion Input Parameters

            #endregion Private Properties

            #region Public Properties

            #region Input/Output Parameters
            public long NumeroDomanda { get { return _NumeroDomanda; } set { _NumeroDomanda = value; } }
            public string StatoPensione { get { return _StatoPensione; } set { _StatoPensione = value; } }
            public string VecchiaMatricola { get { return _VecchiaMatricola; } set { _VecchiaMatricola = value; } }
            public string SedeDiversa { get; set; }
            #endregion Input/Output Parameters

            #region Input Parameters

            public string NuovaMatricola { get { return _NuovaMatricola; } set { _NuovaMatricola = value; } }
            public Utility.TipoOperazione? TipoOperazione { get { return _TipoOperazione; } set { _TipoOperazione = value; } }
            public Utility.TipoAppartenenza? TipoAppOperatore { get { return _TipoAppOperatore; } set { _TipoAppOperatore = value; } }
            public Utility.Ruolo? Ruolo { get { return _Ruolo; } set { _Ruolo = value; } }
            public int? Sede { get { return _Sede; } set { _Sede = value; } }
            #endregion Input Parameters

            #endregion Public Properties
        }
        #endregion nested class
    }
}
