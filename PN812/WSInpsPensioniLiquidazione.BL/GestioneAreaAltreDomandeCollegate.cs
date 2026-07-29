using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaAltreDomandeCollegate
    {
        public static void GetAreaDomandeCollegate(GestionePensione.DatiPensione datiPensione, out List<Entity.DomandeCollegate> listaDomandeCollegate, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            listaDomandeCollegate = new List<Entity.DomandeCollegate>();

            GestioneAnagrafica.DatiAnagrafici anagraficaDanteCausa = null;
            BLCommon.GestioneDanteCausa.GetAnagraficaDanteCausabyIdPensione(datiPensione.Id, out anagraficaDanteCausa);

            if (anagraficaDanteCausa != null)
            {
                // Recupero le domande da GetDomandePerCodFiscRelSogg
                List<ServiceReferences.WebDom.DatiDomanda> elencoDatiDomanda = null;
                GestioneWebDom.GetDomandePerCodiceFiscale(anagraficaDanteCausa.CodiceFiscale, "DA", out elencoDatiDomanda, out messaggioVideo);

                if (elencoDatiDomanda != null && elencoDatiDomanda.Count > 0)
                {
                    BLCommon.GestioneDanteCausa.DatiDanteCausa datiDanteCausa = null;
                    BLCommon.GestioneDanteCausa.GetDanteCausabyIdPensione(datiPensione.Id, out datiDanteCausa);

                    List<GestioneDecodifica.Prodotto> elencoProdotto = null;
                    GestioneDecodifica.GetProdotto(out elencoProdotto);

                    List<GestioneDecodifica.DecSituazione> elencoSituazione = null;
                    GestioneDecodifica.GetDecodificaSituazione(out elencoSituazione);

                    foreach (var datiDomanda in elencoDatiDomanda)
                    {
                        Entity.DomandeCollegate domandaCollegata = new Entity.DomandeCollegate();
                        if (datiDomanda != null && datiDomanda.Dati != null)
                        {
                            if (datiDomanda.Dati.Domanda != null && datiDomanda.Dati.Domanda.Count > 0)
                            {
                                long numeroDomanda = 0;
                                long.TryParse(datiDomanda.Dati.Domanda[0].NumDomanda, out numeroDomanda);
                                domandaCollegata.NumeroDomanda = numeroDomanda;

                                // Salto la domanda corrente
                                if (numeroDomanda == datiPensione.NDomus)
                                    continue;
                            }

                            if (datiDomanda.Dati.Istanza != null && datiDomanda.Dati.Istanza.Count > 0)
                            {
                                if (elencoProdotto != null && elencoProdotto.Count > 0)
                                {
                                    GestioneDecodifica.Prodotto prodotto = elencoProdotto.Find(x => x.CodProdotto == datiDomanda.Dati.Istanza[0].CodProdotto);
                                    domandaCollegata.Prodotto = prodotto != null ? prodotto.DescProdotto : string.Empty;
                                }
                            }

                            GestionePensione.DatiPensione datiPensioneDomandaWebDom = null;
                            GestionePensione.GetPensioneByNumeroDomandaAndProg(domandaCollegata.NumeroDomanda, null, out datiPensioneDomandaWebDom);

                            if (datiPensioneDomandaWebDom != null)
                            {
                                string decodificaStatoPensione = string.Empty;
                                BLCommon.GestioneDecodifica.GetStatoPensioneById(datiPensioneDomandaWebDom.StatoPensione.GetValueOrDefault(), out decodificaStatoPensione);
                                domandaCollegata.StatoLiqPens = decodificaStatoPensione;
                            }

                            if (datiDomanda.Dati.PensioneGenerata != null && datiDomanda.Dati.PensioneGenerata.Count(x => x.IndAnnullata == "0") > 0)
                            {
                                ServiceReferences.WebDom.DataSetDomanda.PensioneGenerataRow pensioneGenerata = datiDomanda.Dati.PensioneGenerata.FirstOrDefault(x => x.IndAnnullata == "0");
                                if (!string.IsNullOrEmpty(pensioneGenerata.NumCertificatoGenAttuale) && 
                                    !string.IsNullOrEmpty(pensioneGenerata.CodProvinciaGenAttuale) &&
                                    !string.IsNullOrEmpty(pensioneGenerata.CodZonaGenAttuale) && 
                                    !string.IsNullOrEmpty(pensioneGenerata.SiglaCatLav))
                                {
                                    domandaCollegata.PensioneAventeDiritto = new Entity.DomandeCollegate.ChiavePensione();

                                    int numCertificatoGenAttuale = 0;
                                    int.TryParse(pensioneGenerata.NumCertificatoGenAttuale, out numCertificatoGenAttuale);
                                    domandaCollegata.PensioneAventeDiritto.Certificato = numCertificatoGenAttuale.ToString().PadLeft(8, '0');
                                    domandaCollegata.PensioneAventeDiritto.Sede = pensioneGenerata.CodProvinciaGenAttuale.Substring(1, 2) + pensioneGenerata.CodZonaGenAttuale.Substring(1, 2);
                                    domandaCollegata.PensioneAventeDiritto.SiglaCategoria = pensioneGenerata.SiglaCatLav;
                                }
                            }

                            if (datiDanteCausa != null)
                            {
                                if (!string.IsNullOrEmpty(datiDanteCausa.SiglaCategoria) && !string.IsNullOrEmpty(datiDanteCausa.Sede) && datiDanteCausa.Certificato.HasValue)
                                {
                                    domandaCollegata.PensioneRiferimentoDC = new Entity.DomandeCollegate.ChiavePensione();

                                    domandaCollegata.PensioneRiferimentoDC.Certificato = datiDanteCausa.Certificato.Value.ToString();
                                    domandaCollegata.PensioneRiferimentoDC.Sede = datiDanteCausa.Sede;
                                    domandaCollegata.PensioneRiferimentoDC.SiglaCategoria = datiDanteCausa.SiglaCategoria;
                                }
                            }

                            if (datiDomanda.Dati.Fase != null && datiDomanda.Dati.Fase.Count > 0)
                            {
                                GestioneDecodifica.DecSituazione situazione = elencoSituazione.Find(x => x.CodSituazione == datiDomanda.Dati.Fase[datiDomanda.Dati.Fase.Count - 1].CodSituazione);
                                domandaCollegata.StatoWebDom = situazione != null ? situazione.DescSituazione : string.Empty;
                            }

                            listaDomandeCollegate.Add(domandaCollegata);
                        }
                    }
                }
            }

            listaDomandeCollegate = listaDomandeCollegate.OrderBy(x => x.NumeroDomanda).ToList();
        }

        public static void GetAventiDirittoDomandaCollegata(long numeroDomandaAventeDiritto, Entity.ParametriARCA parametriArca, string numDomanda, out Entity.AventiDiritto areaAventiDiritto, 
            out string messaggioVideo)
        {
            areaAventiDiritto = new Entity.AventiDiritto();
            messaggioVideo = string.Empty;

            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            List<GestioneAventiDiritto.AventeDirittoRecuperato> listaAventiDirittoFromWebDom = null;
            string codiceFiscaleTitolareDomanda = string.Empty;

            // Recupero gli Aventi Diritto da WebDom
            GestioneWebDom.GetDomandaPerDomus(numeroDomandaAventeDiritto.ToString(), out datiDomanda, out messaggioVideo);
            if (!string.IsNullOrEmpty(messaggioVideo))
                return;
            if(!GestioneAreaRiepilogo.RecuperaAventiDirittoFromWebDom(datiDomanda, out listaAventiDirittoFromWebDom, out messaggioVideo))
                return;

            if (listaAventiDirittoFromWebDom != null && listaAventiDirittoFromWebDom.Count > 0)
            {
                GestioneARCA.RichiestaARCA richiestaArca = new GestioneARCA.RichiestaARCA();
                richiestaArca.Applicazione = parametriArca.Applicazione;
                richiestaArca.Matricola = parametriArca.Matricola;
                richiestaArca.Provenienza = parametriArca.Provenienza;
                richiestaArca.Ruolo = parametriArca.Ruolo;

                List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = new List<GestioneAventiDiritto.AventiDiritto>();
                List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafiche = new List<GestioneAnagrafica.DatiAnagrafici>();
                
                foreach (var aventeDirittoFromWebDom in listaAventiDirittoFromWebDom)
                {
                    richiestaArca.CodiceFiscaleRichiedente = aventeDirittoFromWebDom.CodiceFiscale;
                    richiestaArca.CodiceFiscale = aventeDirittoFromWebDom.CodiceFiscale;

                    Entity.Anagrafica anagrafica = null;
                    if (!string.IsNullOrEmpty(richiestaArca.CodiceFiscale))
                        if (!GestioneARCA.GetAnagraficaArcaByCodiceFiscale(richiestaArca, numDomanda, out anagrafica, out messaggioVideo) || !string.IsNullOrEmpty(messaggioVideo))
                            return;

                    GestioneAnagrafica.DatiAnagrafici anagraficaDB = new GestioneAnagrafica.DatiAnagrafici();
                    Utility.ValorizzaOggetti(anagrafica, anagraficaDB);
                    GestioneAnagrafica.SalvaAnagrafica(anagraficaDB);

                    GestioneAventiDiritto.AventiDiritto aventeDiritto = new GestioneAventiDiritto.AventiDiritto();
                    aventeDiritto.DecParentelaDA = aventeDirittoFromWebDom.DecParentelaDA;
                    aventeDiritto.IdAnagrafica = anagraficaDB.Id;
                    if (aventeDirittoFromWebDom.IsTitolare)
                        codiceFiscaleTitolareDomanda = aventeDirittoFromWebDom.CodiceFiscale;

                    listaAventiDiritto.Add(aventeDiritto);
                    listaAnagrafiche.Add(anagraficaDB);
                }

                GestioneAventiDiritto.SortAventiDiritto(codiceFiscaleTitolareDomanda, ref listaAventiDiritto, listaAnagrafiche);

                areaAventiDiritto.ListaAventiDiritto = listaAventiDiritto;
                areaAventiDiritto.ListaAnagrafiche = listaAnagrafiche;
            }
        }
    }
}
