using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Configuration;

namespace INPS.Pensioni.Liquidazione
{
    public class GestionePuliziaDomanda
    {
        public static void GetPuliziaDomandaByDomanda(long numeroDomanda, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo, Utility.Ruolo ruolo,
            out Entity.PuliziaDomanda entityPuliziaDomanda, out string sedeDiversa, out bool IsPuliziaDisponibile, out string messaggioVideo)
        {
            entityPuliziaDomanda = null;
            IsPuliziaDisponibile = false;
            messaggioVideo = string.Empty;
            sedeDiversa = string.Empty;

            ServiceReferences.WebDom.DatiDomanda domandaWebdom = null;
            GestioneWebDom.GetDomandaPerDomus(numeroDomanda.ToString(), out domandaWebdom, out messaggioVideo);

            if (string.IsNullOrEmpty(messaggioVideo))
            {
                if (!ControlPuliziaDomanda(domandaWebdom, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, out sedeDiversa, out messaggioVideo))
                    if(string.IsNullOrEmpty(sedeDiversa) || ruolo != Utility.Ruolo.AMMINISTRATORE)
                        return;

                if (domandaWebdom != null && domandaWebdom.Dati != null && domandaWebdom.Dati.Attivita != null && domandaWebdom.Dati.Attivita.Count > 0)
                {
                    ValorizzaEntity(domandaWebdom, out entityPuliziaDomanda);
                    GestioneWebDom.CodiceAttivita codAttivita = (GestioneWebDom.CodiceAttivita)Enum.Parse(typeof(GestioneWebDom.CodiceAttivita), domandaWebdom.Dati.Attivita.Last().CodAttivita);

                    GestionePensione.DatiPensione datiPensione = null;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);
                    if (datiPensione != null)
                    {
                        IsPuliziaDisponibile = false;
                        messaggioVideo = "La domanda è presente sul database";
                        return;
                    }
                   
                    switch (codAttivita)
                    {
                        case GestioneWebDom.CodiceAttivita.InAcquisizione:
                        case GestioneWebDom.CodiceAttivita.AttesaCalcolo:
                        case GestioneWebDom.CodiceAttivita.CalcoloErrato:
                            break;
                        default:
                            {
                                IsPuliziaDisponibile = false;
                                return;
                            }
                    }

                    if (entityPuliziaDomanda.DataFine != null)
                    {
                        IsPuliziaDisponibile = false;
                        return;
                    }

                    string codTipoProvvedimento = domandaWebdom.Dati.Attivita.Last()["CodTipoProvvedimento"] != DBNull.Value ? domandaWebdom.Dati.Attivita.Last().CodTipoProvvedimento : string.Empty;
                    if (codTipoProvvedimento != "017")
                    {
                        IsPuliziaDisponibile = false;
                        return;
                    }
  
                    IsPuliziaDisponibile = true;
                }
            }
        }

        public static void EseguiPuliziaDomandaByDomanda(long numeroDomanda, string matricolaOperatore, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo,
            out string sedeDiversa, out Entity.PuliziaDomanda entityPuliziaDomanda, out string messaggioVideo)
        {
            entityPuliziaDomanda = null;
            messaggioVideo = string.Empty;
            sedeDiversa = string.Empty;

            ServiceReferences.WebDom.DatiDomanda domandaWebdom = null;
            GestioneWebDom.GetDomandaPerDomus(numeroDomanda.ToString(), out domandaWebdom, out messaggioVideo);

            if (domandaWebdom != null)
            {
                if (!ControlPuliziaDomanda(domandaWebdom, sedeOperatore, centroOperativoOperatore, tipoAppRuolo, out sedeDiversa, out messaggioVideo))
                    return;

                if (domandaWebdom.Dati != null && domandaWebdom.Dati.Attivita != null && domandaWebdom.Dati.Attivita.Count > 0)
                {
                    ValorizzaEntity(domandaWebdom, out entityPuliziaDomanda);
                    GestioneWebDom.CodiceAttivita codAttivita = (GestioneWebDom.CodiceAttivita)Enum.Parse(typeof(GestioneWebDom.CodiceAttivita), domandaWebdom.Dati.Attivita.Last().CodAttivita);

                    GestionePensione.DatiPensione datiPensione = null;
                    GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

                    if (datiPensione != null)
                    {
                        messaggioVideo = "La domanda è presente sul database";
                        return;
                    }

                    if (codAttivita == GestioneWebDom.CodiceAttivita.InAcquisizione ||
                        codAttivita == GestioneWebDom.CodiceAttivita.AttesaCalcolo ||
                        codAttivita == GestioneWebDom.CodiceAttivita.CalcoloErrato)
                    {
                        string codTipoProvvedimento = domandaWebdom.Dati.Attivita.Last()["CodTipoProvvedimento"] != DBNull.Value ? domandaWebdom.Dati.Attivita.Last().CodTipoProvvedimento : string.Empty;
                        if (codTipoProvvedimento == "017")
                        {
                            if (entityPuliziaDomanda.DataFine == null)
                            {
                                GestioneWebDom.ChiusuraUltimaAttivita(datiPensione, matricolaOperatore, sedeOperatore, out messaggioVideo);
                                if (!string.IsNullOrEmpty(messaggioVideo))
                                    return;
                            }
                            else
                            {
                                messaggioVideo = "L'ultima attività risulta già chiusa";
                                return;
                            }
                        }
                        else
                        {
                            messaggioVideo = "Non è possibile chiudere l'ultima attività.";
                            return;
                        }
                    }
                    else
                    {
                        messaggioVideo = "L'ultima attività è diversa da \"InAcquisizione\", \"AttesaCalcolo\", \"CalcoloErrato\"";
                        return;
                    }
                }
            }
        }

        private static void ValorizzaEntity(ServiceReferences.WebDom.DatiDomanda domandaWebdom, out Entity.PuliziaDomanda entityPuliziaDomanda)
        {
            entityPuliziaDomanda = null;
            if (domandaWebdom != null && domandaWebdom.Dati != null && domandaWebdom.Dati.Attivita != null && domandaWebdom.Dati.Attivita.Count > 0 && !string.IsNullOrEmpty(domandaWebdom.Dati.Attivita.Last().CodAttivita))
            {
                entityPuliziaDomanda = new Entity.PuliziaDomanda();
                GestioneWebDom.CodiceAttivita codAttivita = (GestioneWebDom.CodiceAttivita)Enum.Parse(typeof(GestioneWebDom.CodiceAttivita), domandaWebdom.Dati.Attivita.Last().CodAttivita);
                entityPuliziaDomanda.Attivita = Utility.GetDescription(codAttivita) + " - " + codAttivita.ToString();
                entityPuliziaDomanda.DataInizio = domandaWebdom.Dati.Attivita.Last()["DataInizio"] != DBNull.Value ? Utility.DataFromString(domandaWebdom.Dati.Attivita.Last().DataInizio, Utility.FormatoData.AAAAmmGG) : (DateTime?)null;
                entityPuliziaDomanda.DataFine = domandaWebdom.Dati.Attivita.Last()["DataFine"] != DBNull.Value ? Utility.DataFromString(domandaWebdom.Dati.Attivita.Last().DataFine, Utility.FormatoData.AAAAmmGG) : (DateTime?)null;
            }
        }

        private static bool ControlPuliziaDomanda(ServiceReferences.WebDom.DatiDomanda domandaWebdom, short sedeOperatore, short centroOperativoOperatore, Utility.TipoAppartenenza tipoAppRuolo,
            out string sedeDiversa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            sedeDiversa = string.Empty;

            if (domandaWebdom == null)
            {
                messaggioVideo = "Domanda non presente su WebDom";
                return false;
            }

            if (domandaWebdom.Dati != null && domandaWebdom.Dati.Istanza != null && domandaWebdom.Dati.Istanza.Count > 0)
            {
                bool? IndConvInt = (!string.IsNullOrEmpty(domandaWebdom.Dati.Istanza[0].IndConvInt) && domandaWebdom.Dati.Istanza[0].IndConvInt.Trim() == "1") ? true : (!string.IsNullOrEmpty(domandaWebdom.Dati.Istanza[0].IndConvInt) && domandaWebdom.Dati.Istanza[0].IndConvInt.Trim() == "0") ? false : (bool?)null;

                //Controllo se la domanda è lavorabile da parte dell'operatore
                Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(IndConvInt, domandaWebdom.Dati.Istanza[0].CodGestione);
                if (tipoAppRuolo != tipoAppartenenza)
                {
                    messaggioVideo = "Ruolo Utente non abilitato alla lavorazione della domanda.";
                    return false;
                }
            }

            if (domandaWebdom.Dati == null || domandaWebdom.Dati.Attivita == null || domandaWebdom.Dati.Attivita.Count == 0 || string.IsNullOrEmpty(domandaWebdom.Dati.Attivita.Last().CodAttivita))
            {
                messaggioVideo = "Nessuna attività presente su WebDom per la domanda.";
                return false;
            }

            GestioneWebDom.CodiceAttivita codAttivita = (GestioneWebDom.CodiceAttivita)Enum.Parse(typeof(GestioneWebDom.CodiceAttivita), domandaWebdom.Dati.Attivita.Last().CodAttivita);
            if (!Enum.IsDefined(typeof(GestioneWebDom.CodiceAttivita), codAttivita))
            {
                messaggioVideo = "Ultima attività WebDom non gestita dall'applicativo.";
                return false;
            }

            if (ConfigurationManager.AppSettings["BypassControlloSedi"] == null ||
                        ConfigurationManager.AppSettings["BypassControlloSedi"] != "SI")
            {
                //controllo sede operatore - sede domanda
                short sedeDomanda = 0;
                short centroOperativoDomanda = 0;
                if (!GestioneAreaRiepilogo.CheckSedi(domandaWebdom, sedeOperatore, centroOperativoOperatore, out sedeDomanda, out centroOperativoDomanda))
                {
                    messaggioVideo = "La sede dell'operatore non coincide con la sede della domanda selezionata (" +
                        sedeDomanda.ToString().PadLeft(4, '0') + centroOperativoDomanda.ToString().PadLeft(2, '0') + ").";
                    sedeDiversa = sedeDomanda.ToString().PadLeft(4, '0') + centroOperativoDomanda.ToString().PadLeft(2, '0');
                    return false;
                }
            }

            return true;
        }
    }
}
