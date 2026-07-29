using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaTipologieNonAbilitate
    {
        public static void GetAllTipologieNonAbilitate(out List<GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate> elencoTipologieNonAbilitate)
        {
            elencoTipologieNonAbilitate = null;
            GestioneTipologieNonAbilitate.GetAllTipologieNonAbilitate(out elencoTipologieNonAbilitate);
        }

        public static void StoreTipologieNonAbilitate(GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate areaTipologieNonAbilitate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlTipologieNonAbilitate(areaTipologieNonAbilitate, out messaggioVideo))
                return;

            GestioneTipologieNonAbilitate.SalvaTipologieNonAbilitate(areaTipologieNonAbilitate);
        }

        public static void DeleteTipologieNonAbilitate(GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate areaTipologieNonAbilitate)
        {
            GestioneTipologieNonAbilitate.EliminaTipologieNonAbilitate(areaTipologieNonAbilitate);
        }

        public static void GetListaGruppo(out List<GestioneTipologieNonAbilitate.Gruppo> listaGruppo)
        {
            listaGruppo = new List<GestioneTipologieNonAbilitate.Gruppo>();
            List<GestioneDecodifica.Gruppo> listaGruppoDB = null;
            GestioneDecodifica.GetGruppo(out listaGruppoDB);
            if (listaGruppoDB != null)
            {
                foreach (GestioneDecodifica.Gruppo gruppoDB in listaGruppoDB)
                {
                    GestioneTipologieNonAbilitate.Gruppo gruppo = new GestioneTipologieNonAbilitate.Gruppo();
                    Utility.ValorizzaOggetti(gruppoDB, gruppo);
                    listaGruppo.Add(gruppo);
                }
            }
        }

        public static void GetListaProdotto(out List<GestioneTipologieNonAbilitate.Prodotto> listaProdotto)
        {
            listaProdotto = new List<GestioneTipologieNonAbilitate.Prodotto>();
            List<GestioneDecodifica.Prodotto> listaProdottoDB = null;
            GestioneDecodifica.GetProdotto(out listaProdottoDB);
            if (listaProdottoDB != null)
            {
                foreach (GestioneDecodifica.Prodotto prodottoDB in listaProdottoDB)
                {
                    GestioneTipologieNonAbilitate.Prodotto prodotto = new GestioneTipologieNonAbilitate.Prodotto();
                    Utility.ValorizzaOggetti(prodottoDB, prodotto);
                    listaProdotto.Add(prodotto);
                }
            }
        }

        public static void GetListaTipo(out List<GestioneTipologieNonAbilitate.Tipo> listaTipo)
        {
            listaTipo = new List<GestioneTipologieNonAbilitate.Tipo>();
            List<GestioneDecodifica.Tipo> listaTipoDB = null;
            GestioneDecodifica.GetTipo(out listaTipoDB);
            if (listaTipoDB != null)
            {
                foreach (GestioneDecodifica.Tipo tipoDB in listaTipoDB)
                {
                    GestioneTipologieNonAbilitate.Tipo tipo = new GestioneTipologieNonAbilitate.Tipo();
                    Utility.ValorizzaOggetti(tipoDB, tipo);
                    listaTipo.Add(tipo);
                }
            }
        }

        public static void GetListaFiltro(out List<GestioneTipologieNonAbilitate.Filtro> listaFiltro)
        {
            listaFiltro = new List<GestioneTipologieNonAbilitate.Filtro>();
            List<GestioneDecodifica.GestioneCodiceTipoRichiesta> ListaCodTipoRichiesta = null;
            GestioneDecodifica.GetGestioneCodeTipoRichiesta(out ListaCodTipoRichiesta);
            if (ListaCodTipoRichiesta != null)
            {
                //elimino dalla lista tutti i record che hanno il Filtro duplicato
                ListaCodTipoRichiesta = ListaCodTipoRichiesta.GroupBy(x => x.Filtro).Select(x => x.First()).ToList();

                foreach (GestioneDecodifica.GestioneCodiceTipoRichiesta codfiltro in ListaCodTipoRichiesta)
                {
                    GestioneTipologieNonAbilitate.Filtro filtro = new GestioneTipologieNonAbilitate.Filtro();
                    filtro.Codice = codfiltro.Filtro;
                    filtro.Descrizione = codfiltro.DescTipoRichiesta;
                    listaFiltro.Add(filtro);
                }
            }
        }

        private static bool ControlTipologieNonAbilitate(GestioneTipologieNonAbilitate.DatiTipologieNonAbilitate areaTipologieNonAbilitate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (areaTipologieNonAbilitate == null)
            {
                messaggioVideo = "Nessuta tipologia non abilitata da salvare";
                return false;
            }

            if (string.IsNullOrEmpty(areaTipologieNonAbilitate.TipoApp))
            {
                messaggioVideo = "'Tipo Appartenenza' non valorizzato";
                return false;
            }

            if (areaTipologieNonAbilitate.TipoApp == "FS")
            {
                if (string.IsNullOrEmpty(areaTipologieNonAbilitate.Fondo))
                {
                    messaggioVideo = "'Fondo' non valorizzato";
                    return false;
                }

                List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensione = null;
                GestioneDecodifica.GetCategoriePensione(out elencoCategoriePensione);
                if (elencoCategoriePensione != null)
                {
                    elencoCategoriePensione = elencoCategoriePensione.FindAll(x => x.AppartenenzaCatPensione == areaTipologieNonAbilitate.TipoApp);
                    if (elencoCategoriePensione.Count > 0)
                    {
                        int index = 0;

                        //utilizzo la substring in quanto, sulla tabella DecCatPensione, la sigla categoria, per il fondo PT è valorizzada nel seguente modo: IPT - VPT - SPT
                        if (areaTipologieNonAbilitate.Fondo == "PT")
                            index = elencoCategoriePensione.FindIndex(x => x.SiglaCatPensione.Trim().Substring(x.SiglaCatPensione.Trim().Length - 2, 2) == areaTipologieNonAbilitate.Fondo);
                        else if (areaTipologieNonAbilitate.Fondo != "INPDAP")
                            index = elencoCategoriePensione.FindIndex(x => x.SiglaCatPensione.Trim() == areaTipologieNonAbilitate.Fondo);

                        if (index < 0)
                        {
                            messaggioVideo = "Il 'Fondo' che si sta tentando di salvare non è valido";
                            return false;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(areaTipologieNonAbilitate.Gruppo))
            {
                messaggioVideo = "'Gruppo' non valorizzato";
                return false;
            }

            if (string.IsNullOrEmpty(areaTipologieNonAbilitate.Prodotto))
            {
                messaggioVideo = "'Prodotto' non valorizzato";
                return false;
            }

            if (string.IsNullOrEmpty(areaTipologieNonAbilitate.Tipo))
            {
                messaggioVideo = "'Tipo' non valorizzato";
                return false;
            }

            if (areaTipologieNonAbilitate.Gruppo.Length != 4)
            {
                messaggioVideo = "Il 'Gruppo' deve contenere 4 cifre";
                return false;
            }

            if (!areaTipologieNonAbilitate.Prodotto.Equals("ALL") && areaTipologieNonAbilitate.Prodotto.Length != 4)
            {
                messaggioVideo = "Il 'Prodotto' deve contenere 4 cifre";
                return false;
            }

            if (!areaTipologieNonAbilitate.Tipo.Equals("ALL") && areaTipologieNonAbilitate.Tipo.Length != 4)
            {
                messaggioVideo = "Il 'Tipo' deve contenere 4 cifre";
                return false;
            }

            if (!string.IsNullOrEmpty(areaTipologieNonAbilitate.Filtro) && areaTipologieNonAbilitate.Filtro.Length > 3)
            {
                messaggioVideo = "Il 'Filtro' può contenere al massimo 3 caratteri";
                return false;
            }

            if (areaTipologieNonAbilitate.Gruppo.Equals("ALL"))
            {
                messaggioVideo = "Il 'Gruppo' non può essere valorizzato con 'ALL'";
                return false;
            }

            List<GestioneTipologieNonAbilitate.Gruppo> elencoGruppo = null;
            GestioneAreaTipologieNonAbilitate.GetListaGruppo(out elencoGruppo);
            if (elencoGruppo != null)
            {
                int index = elencoGruppo.FindIndex(x => x.CodGruppo == areaTipologieNonAbilitate.Gruppo);
                if (index < 0)
                {
                    messaggioVideo = "Il 'Gruppo' che si sta tentando di salvare non è valido";
                    return false;
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza del prodotto nella lista, in quanto ALL contempla tutti i prodotti
            if (!areaTipologieNonAbilitate.Prodotto.ToUpperInvariant().Equals("ALL"))
            {
                List<GestioneTipologieNonAbilitate.Prodotto> elencoProdotto = null;
                GestioneAreaTipologieNonAbilitate.GetListaProdotto(out elencoProdotto);
                if (elencoProdotto != null)
                {
                    int index = elencoProdotto.FindIndex(x => x.CodProdotto == areaTipologieNonAbilitate.Prodotto);
                    if (index < 0)
                    {
                        messaggioVideo = "Il 'Prodotto' che si sta tentando di salvare non è valido";
                        return false;
                    }
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza del tipo nella lista, in quanto ALL contempla tutti i tipi
            if (!areaTipologieNonAbilitate.Tipo.ToUpperInvariant().Equals("ALL"))
            {
                List<GestioneTipologieNonAbilitate.Tipo> elencoTipo = null;
                GestioneAreaTipologieNonAbilitate.GetListaTipo(out elencoTipo);
                if (elencoTipo != null)
                {
                    int index = elencoTipo.FindIndex(x => x.CodTipo == areaTipologieNonAbilitate.Tipo);
                    if (index < 0)
                    {
                        messaggioVideo = "Il 'Tipo' che si sta tentando di salvare non è valido";
                        return false;
                    }
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza del filtro nella lista, in quanto ALL contempla tutti i filtri
            if (!areaTipologieNonAbilitate.Filtro.ToUpperInvariant().Equals("ALL"))
            {
                List<GestioneTipologieNonAbilitate.Filtro> elencoFiltro = null;
                GestioneAreaTipologieNonAbilitate.GetListaFiltro(out elencoFiltro);
                if (elencoFiltro != null)
                {
                    int index = elencoFiltro.FindIndex(x => (x.Codice == null ? string.Empty : x.Codice) == areaTipologieNonAbilitate.Filtro);
                    if (index < 0)
                    {
                        messaggioVideo = "Il 'Filtro' che si sta tentando di salvare non è valido";
                        return false;
                    }
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza della sigla categoria nella lista, in quanto ALL contempla tutte le categorie
            if (!areaTipologieNonAbilitate.SiglaCategoria.ToUpperInvariant().Equals("ALL"))
            {
                List<string> elencoSiglaCategoria = null;
                Utility.GetListaSigleCategoriePerTipoApp(out elencoSiglaCategoria, areaTipologieNonAbilitate.TipoApp);
                if (elencoSiglaCategoria != null)
                {
                    if (!elencoSiglaCategoria.Contains(areaTipologieNonAbilitate.SiglaCategoria))
                    {
                        messaggioVideo = "La 'Sigla Categoria' che si sta tentando di salvare non è valida";
                        return false;
                    }
                }
            }

            return true;
        }
    }
}