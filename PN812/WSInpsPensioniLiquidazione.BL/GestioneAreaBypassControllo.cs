using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaBypassControllo
    {
        public static void GetDecBypassControlloByTipoApp(string tipoApp, out List<GestioneBypassControllo.DatiDecBypassControllo> lstDecBypassControllo)
        {
            lstDecBypassControllo = new List<GestioneBypassControllo.DatiDecBypassControllo>();
            GestioneBypassControllo.GetDecBypassControlloByTipoApp(tipoApp, out lstDecBypassControllo);
        }

        public static void GetAllBypassControlloByTipoApp(string tipoApp, out List<GestioneBypassControllo.DatiBypassControllo> elencoBypassControllo)
        {
            elencoBypassControllo = null;
            GestioneBypassControllo.GetAllBypassControlloByTipoApp(tipoApp, out elencoBypassControllo);
        }

        public static void StoreBypassControllo(GestioneBypassControllo.DatiBypassControllo datiBypassControllo, Utility.TipoAppartenenza tipoApp, out string messaggio)
        {
            messaggio = string.Empty;
            //controlli sulla correttezza dei dati
            if (!ControlsBypassControllo(datiBypassControllo, tipoApp, out messaggio))
                return;
            //controllo che non esiste già a db un controllo per quella NDomus
            if (!ControlBypassNonPresente(datiBypassControllo, out messaggio))
            {
                return;
            }

            GestioneBypassControllo.SalvaBypassControllo(datiBypassControllo);
        }

        public static void DeleteBypassControlloById(long id)
        {
            GestioneBypassControllo.EliminaBypassControlloById(id);
        }

        public static void DeleteAllBypassControlloByDomus(long NDomus)
        {
            GestioneBypassControllo.DeleteAllBypassControlloDinamiciByDomus(NDomus);
        }

        private static bool IsChiavePensioneValorizzata(GestioneBypassControllo.DatiBypassControllo datiBypassControllo)
        {
            return datiBypassControllo.CodCategoria != null && datiBypassControllo.CodiceSede != null && datiBypassControllo.NCertificato != null;
        }

        private static bool ControlsBypassControllo(GestioneBypassControllo.DatiBypassControllo datiBypassControllo, Utility.TipoAppartenenza tipoApp, out string messaggio)
        {
            messaggio = string.Empty;
            if (datiBypassControllo.Matricola == null || datiBypassControllo.Matricola.Length != 8)
            {
                messaggio = "La matricolo inserita è erratta";
                return false;
            }

            if (IsChiavePensioneValorizzata(datiBypassControllo))
            {
                if (datiBypassControllo.CodCategoria.ToString().PadLeft(3, '0').Length != 3)
                {
                    messaggio = "Il Codice Categoria della chiave pensione deve essere di 3 caratteri";
                    return false;
                }

                List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensione = null;
                GestioneDecodifica.GetCategoriePensione(out elencoCategoriePensione);
                if (elencoCategoriePensione != null && elencoCategoriePensione.Count > 0)
                {
                    GestioneDecodifica.CategoriaPensione categoriaPensione = null;
                    categoriaPensione = elencoCategoriePensione.
                        Where(elem => elem.CodCatPensione == (datiBypassControllo.CodCategoria.PadLeft(4, '0'))).FirstOrDefault();

                    if (categoriaPensione != null)
                    {
                        if (!string.IsNullOrEmpty(categoriaPensione.AppartenenzaCatPensione) && tipoApp.ToString() != categoriaPensione.AppartenenzaCatPensione.Trim())
                        {
                            messaggio = "Il tipo appartenenza della categoria pensione non corrisponde al ruolo dell'utente ";
                            return false;
                        }
                    }
                    else
                    {
                        messaggio = "Codice Categoria non corretto";
                        return false;
                    }
                }
            }
            else
            {
                if (datiBypassControllo.NDomus.ToString().Length != 13)
                {
                    messaggio = "Il Numero Domanda deve essere di 13 caratteri";
                    return false;
                }
                GestionePensione.DatiPensione datiPensione = null;
                GestionePensione.GetPensioneByNumeroDomandaAndProg(datiBypassControllo.NDomus.GetValueOrDefault(), null, out datiPensione);
                if (datiPensione != null && tipoApp != Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione))
                {
                    messaggio = "Il tipo appartenenza della domanda non corrisponde al ruolo dell'utente ";
                    return false;
                }
            }

            if (datiBypassControllo.IdDecBypassControllo == 0)
            {
                messaggio = "Inserire un tipo di controllo da bypassare";
                return false;
            }

            return true;
        }

        private static bool ControlBypassNonPresente(GestioneBypassControllo.DatiBypassControllo datiBypassControllo, out string messaggio)
        {
            messaggio = string.Empty;
            bool ret = true;
            GestioneBypassControllo.DatiBypassControllo bypass = null;
            if (IsChiavePensioneValorizzata(datiBypassControllo))
                GestioneBypassControllo.GetBypassControlloByChiavePensioneAndIdDec(datiBypassControllo.CodCategoria, datiBypassControllo.CodiceSede, datiBypassControllo.NCertificato, datiBypassControllo.IdDecBypassControllo, out bypass);
            else
                GestioneBypassControllo.GetBypassControlloByNDomusAndIdDec(datiBypassControllo.NDomus, datiBypassControllo.IdDecBypassControllo, out bypass);

            if (bypass != null)
            {
                ret = false;
                messaggio = "Attenzione si è già salvato questo bypass per la domanda corrente";
            }
            return ret;
        }
    }
}
