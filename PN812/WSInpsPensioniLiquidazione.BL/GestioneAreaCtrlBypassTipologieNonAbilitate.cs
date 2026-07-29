using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaCtrlBypassTipologieNonAbilitate
    {
        public static void StoreCtrlBypassTipologieNonAbilitate(BLCommon.GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate areaCtrlBypassTipologieNonAbilitate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            if (!ControlCtrlBypassTipologieNonAbilitate(areaCtrlBypassTipologieNonAbilitate, out messaggioVideo))
                return;

            BLCommon.GestioneCtrlBypassTipologieNonAbilitate.SalvaCtrlBypassTipologieNonAbilitate(areaCtrlBypassTipologieNonAbilitate);
        }

        public static bool ControlCtrlBypassTipologieNonAbilitate(GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate areaCtrlBypassTipologieNonAbilitate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (areaCtrlBypassTipologieNonAbilitate == null)
            {
                messaggioVideo = "Nessun bypass da salvare.";
                return false;
            }

            if (string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Tipologia))
            {
                messaggioVideo = "'Tipo Appartenenza' non valorizzato.";
                return false;
            }

            List<string> elencoSigleCategoriePensione = null;
            Utility.GetListaSigleCategoriePerTipoApp(out elencoSigleCategoriePensione, areaCtrlBypassTipologieNonAbilitate.Tipologia);


            if (areaCtrlBypassTipologieNonAbilitate.Tipologia == "FS")
            {
                if (string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Fondo))
                {
                    messaggioVideo = "'Fondo' non valorizzato";
                    return false;
                }

                if (!areaCtrlBypassTipologieNonAbilitate.Fondo.Equals("ALL"))
                {
                    List<GestioneDecodifica.CategoriaPensione> elencoCategoriePensione = null;
                    GestioneDecodifica.GetCategoriePensione(out elencoCategoriePensione);

                    if (elencoCategoriePensione != null)
                    {
                        elencoCategoriePensione = elencoCategoriePensione.FindAll(x => x.AppartenenzaCatPensione == areaCtrlBypassTipologieNonAbilitate.Tipologia);
                        if (elencoCategoriePensione.Count > 0)
                        {
                            GestioneDecodifica.CategoriaPensione categoria;

                            //utilizzo la substring in quanto, sulla tabella DecCatPensione, la sigla categoria, per il fondo PT è valorizzada nel seguente modo: IPT - VPT - SPT
                            if (areaCtrlBypassTipologieNonAbilitate.Fondo.Trim().ToUpperInvariant() == "PT")
                                categoria = elencoCategoriePensione.Find(x => x.SiglaCatPensione.Trim().Substring(x.SiglaCatPensione.Trim().Length - 2, 2).ToUpperInvariant() == areaCtrlBypassTipologieNonAbilitate.Fondo.ToUpperInvariant());
                            else
                                categoria = elencoCategoriePensione.Find(x => x.SiglaCatPensione.Trim().ToUpperInvariant() == areaCtrlBypassTipologieNonAbilitate.Fondo.ToUpperInvariant());

                            if (areaCtrlBypassTipologieNonAbilitate.Fondo != "INPDAP" && categoria == null)
                            {
                                messaggioVideo = "Il 'Fondo' che si sta tentando di salvare non è valido";
                                return false;
                            }

                            if (!areaCtrlBypassTipologieNonAbilitate.Categoria.Equals("ALL"))
                            {
                                Utility.TipoFondo? tipoFondo = Utility.GetTipoFondoByCategoria(Utility.TipoAppartenenza.FS, areaCtrlBypassTipologieNonAbilitate.Categoria);
                                if (tipoFondo.HasValue && tipoFondo.Value.ToString().Trim().ToUpperInvariant() != areaCtrlBypassTipologieNonAbilitate.Fondo.Trim().ToUpperInvariant())
                                {
                                    messaggioVideo = "Il 'Fondo' e la 'Categoria' non sono compatibili.";
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Fondo))
                {
                    messaggioVideo = "Il 'Fondo' non deve essere valorizzato.";
                    return false;
                }
            }

            if (string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Gruppo))
            {
                messaggioVideo = "'Gruppo' non valorizzato";
                return false;
            }

            if (string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Prodotto))
            {
                messaggioVideo = "'Prodotto' non valorizzato";
                return false;
            }

            if (string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Tipo))
            {
                messaggioVideo = "'Tipo' non valorizzato";
                return false;
            }

            if (string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Categoria))
            {
                messaggioVideo = "'Categoria' non valorizzata";
                return false;
            }

            if (areaCtrlBypassTipologieNonAbilitate.Sede == 0)
            {
                messaggioVideo = "'Sede' non valorizzata";
                return false;
            }

            if (areaCtrlBypassTipologieNonAbilitate.Gruppo.Length != 4)
            {
                messaggioVideo = "Il 'Gruppo' deve contenere 4 cifre";
                return false;
            }

            if (!areaCtrlBypassTipologieNonAbilitate.Prodotto.Equals("ALL") && areaCtrlBypassTipologieNonAbilitate.Prodotto.Length != 4)
            {
                messaggioVideo = "Il 'Prodotto' deve contenere 4 cifre";
                return false;
            }

            if (!areaCtrlBypassTipologieNonAbilitate.Tipo.Equals("ALL") && areaCtrlBypassTipologieNonAbilitate.Tipo.Length != 4)
            {
                messaggioVideo = "Il 'Tipo' deve contenere 4 cifre";
                return false;
            }

            if (!string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Filtro) && areaCtrlBypassTipologieNonAbilitate.Filtro.Length != 3)
            {
                messaggioVideo = "Il 'Filtro' deve essere di 3 caratteri";
                return false;
            }

            if (!string.IsNullOrEmpty(areaCtrlBypassTipologieNonAbilitate.Categoria) && areaCtrlBypassTipologieNonAbilitate.Categoria.Length > 8)
            {
                messaggioVideo = "La 'Categoria' può contenere al massimo 8 caratteri.";
                return false;
            }

            if (areaCtrlBypassTipologieNonAbilitate.Gruppo.Equals("ALL"))
            {
                messaggioVideo = "Il 'Gruppo' non può essere valorizzato con 'ALL'";
                return false;
            }

            List<GestioneTipologieNonAbilitate.Gruppo> elencoGruppo = null;
            GestioneAreaTipologieNonAbilitate.GetListaGruppo(out elencoGruppo);
            if (elencoGruppo != null)
            {
                int index = elencoGruppo.FindIndex(x => x.CodGruppo == areaCtrlBypassTipologieNonAbilitate.Gruppo);
                if (index < 0)
                {
                    messaggioVideo = "Il 'Gruppo' che si sta tentando di salvare non è valido";
                    return false;
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza del prodotto nella lista, in quanto ALL contempla tutti i prodotti
            if (!areaCtrlBypassTipologieNonAbilitate.Prodotto.ToUpperInvariant().Equals("ALL"))
            {
                List<GestioneTipologieNonAbilitate.Prodotto> elencoProdotto = null;
                GestioneAreaTipologieNonAbilitate.GetListaProdotto(out elencoProdotto);
                if (elencoProdotto != null)
                {
                    int index = elencoProdotto.FindIndex(x => x.CodProdotto == areaCtrlBypassTipologieNonAbilitate.Prodotto);
                    if (index < 0)
                    {
                        messaggioVideo = "Il 'Prodotto' che si sta tentando di salvare non è valido";
                        return false;
                    }
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza del tipo nella lista, in quanto ALL contempla tutti i tipi
            if (!areaCtrlBypassTipologieNonAbilitate.Tipo.ToUpperInvariant().Equals("ALL"))
            {
                List<GestioneTipologieNonAbilitate.Tipo> elencoTipo = null;
                GestioneAreaTipologieNonAbilitate.GetListaTipo(out elencoTipo);
                if (elencoTipo != null)
                {
                    int index = elencoTipo.FindIndex(x => x.CodTipo == areaCtrlBypassTipologieNonAbilitate.Tipo);
                    if (index < 0)
                    {
                        messaggioVideo = "Il 'Tipo' che si sta tentando di salvare non è valido";
                        return false;
                    }
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza del filtro nella lista, in quanto ALL contempla tutti i filtri
            if (!areaCtrlBypassTipologieNonAbilitate.Filtro.ToUpperInvariant().Equals("ALL"))
            {
                List<GestioneTipologieNonAbilitate.Filtro> elencoFiltro = null;
                GestioneAreaTipologieNonAbilitate.GetListaFiltro(out elencoFiltro);
                if (elencoFiltro != null)
                {
                    int index = elencoFiltro.FindIndex(x => (x.Codice == null ? string.Empty : x.Codice) == areaCtrlBypassTipologieNonAbilitate.Filtro);
                    if (index < 0)
                    {
                        messaggioVideo = "Il 'Filtro' che si sta tentando di salvare non è valido";
                        return false;
                    }
                }
            }

            //se è stato inserito ALL, non vado a controllare la presenza del filtro nella lista, in quanto ALL contempla tutti i filtri
            if (!areaCtrlBypassTipologieNonAbilitate.Categoria.ToUpperInvariant().Equals("ALL"))
            {
                if (elencoSigleCategoriePensione != null)
                {
                    if (!elencoSigleCategoriePensione.Select(x => x.Trim().ToUpperInvariant()).Contains(areaCtrlBypassTipologieNonAbilitate.Categoria))
                    {
                        messaggioVideo = "La 'Categoria' che si sta tentando di salvare non è valida";
                        return false;
                    }
                }
            }

            if (!Utility.ExistSedeProvinciale(areaCtrlBypassTipologieNonAbilitate.Sede))
            {
                messaggioVideo = "La 'Sede' che si sta tentando di salvare non è valida.";
                return false;
            }

            return true;
        }
    }
}
