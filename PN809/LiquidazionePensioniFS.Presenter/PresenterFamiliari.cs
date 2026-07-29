using System;
using System.Collections.Generic;
using System.Linq;
using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterFamiliari
    {
        public void RicercaAnagraficaByCodiceFiscale(IFamiliari familiari)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                Presenter.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
                richiesta.CodiceFiscale = familiari.codiceFiscale;
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                AreaRispostaRiepilogo.DatiRiepilogoAnagrafica risposta = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                esito = objWS.GetAnagraficaSoggettoByCodiceFiscale(out risposta, richiesta.CodiceFiscale, Utility.GetSedeOperatore(), Utility.GetMatricolaOperatore(), familiari.domanda.NumeroDomanda);
                familiari.areaEsito = esito;
                familiari.areaRiepilogoAnagrafica = risposta;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFamiliari, Errore nel metodo RicercaAnagraficaByCodiceFiscale");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void AggiornaAnagraficaByArca(IFamiliari familiari)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                Presenter.SvrLiquidazione.AreaRichiestaRiepilogo richiesta = new AreaRichiestaRiepilogo();
                richiesta.CodiceFiscale = familiari.codiceFiscale;
                Presenter.SvrLiquidazione.AreaEsito esito = new AreaEsito();
                AreaRispostaRiepilogo.DatiRiepilogoAnagrafica risposta = new AreaRispostaRiepilogo.DatiRiepilogoAnagrafica();
                esito = objWS.AggiornaAnagraficaSoggetto(out risposta, richiesta.CodiceFiscale, Utility.GetSedeOperatore(), Utility.GetMatricolaOperatore(), familiari.domanda.NumeroDomanda);
                familiari.areaEsito = esito;
                familiari.areaRiepilogoAnagrafica = risposta;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFamiliari, Errore nel metodo AggiornaAnagraficaByArca");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void GetFamiliareByNumDomanda(IFamiliari familiari, ITitolarePensione titolarePensione)
        {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient proxy = new ServizioLiquidazioneClient();
            AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
            areaRichiestaDomanda.NumeroDomanda = Int64.Parse(familiari.domanda.NumeroDomanda);
            areaRichiestaDomanda.ProgStorico = familiari.domanda.ProgStorico;

            Anagrafica[] myListanagrafiche = null;
            GestioneAreaFamiliariAreaFamiliare[] mylistfamiliari = null;
            GestioneAreaFamiliariAreaDecFam myAreaDecodifica = null;
            esito = proxy.GetFamiliareByNumeroDomanda(out mylistfamiliari, out myListanagrafiche, out myAreaDecodifica, areaRichiestaDomanda);

            familiari.areaFamiliare = mylistfamiliari.ToList();
            familiari.anagrafica = myListanagrafiche.ToList();
            familiari.areaDecodifica = myAreaDecodifica;
            familiari.HasError = esito.RisultatoOperazione == AreaEsito.TipoEsito.KO ? true : false;
            familiari.ErrorMessage = esito.Messaggio;

            MergeListAnagraficaFamiliare(familiari, titolarePensione);
        }

        public AreaEsito SalvaFamiliari(IFamiliari familiari, ITitolarePensione titolarePensione)
        {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient proxy = new ServizioLiquidazioneClient();
            try
            {
                long numeroDomanda = long.Parse(familiari.domanda.NumeroDomanda);

                SeparateListFamiliariFull(familiari);

                GestioneAreaFamiliariAreaFamiliare[] familiariToSave = familiari.areaFamiliare.ToArray();
                Anagrafica[] anagraficaToSave = familiari.anagrafica.ToArray();
                string[] familiariToDelete = familiari.familiariToDelete.ToArray();
                GestioneFamiliariConsultazioneUnificataANF consultazioneAnf = null;

                if (familiariToSave != null && familiariToSave.Length > 0)
                {
                    foreach (GestioneAreaFamiliariAreaFamiliare fam in familiariToSave)
                    {
                        if (familiariToDelete != null && familiariToDelete.Length > 0)
                            if (Array.Exists(familiariToDelete, x => x == fam.Familiare.CodiceFiscale))
                                Array.Clear(familiariToDelete, Array.IndexOf(familiariToDelete, fam.Familiare.CodiceFiscale, 0), 1);
                    }
                }

                esito = proxy.SalvaFamiliari(numeroDomanda, familiari.codiceFiscale, Utility.GetMatricolaOperatore(), ref familiariToSave, familiariToDelete, ref anagraficaToSave, out consultazioneAnf);

                familiari.areaFamiliare = familiariToSave.ToList();
                familiari.anagrafica = anagraficaToSave.ToList();
                familiari.HasError = esito.RisultatoOperazione == AreaEsito.TipoEsito.KO ? true : false;
                familiari.ErrorMessage = esito.Messaggio;
                familiari.consultazioneANF = consultazioneAnf;

                MergeListAnagraficaFamiliare(familiari, titolarePensione);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFamiliari, Errore nel metodo SalvaFamiliari");
            }
            finally
            {
                Utility.CloseClient(proxy);
            }

            return esito;
        }

        public void CancellaFamiliari(IFamiliari familiari, ITitolarePensione titolarePensione)
        {
            AreaEsito esito = new AreaEsito();
            ServizioLiquidazioneClient proxy = new ServizioLiquidazioneClient();
            try
            {
                long numeroDomanda = long.Parse(familiari.domanda.NumeroDomanda);
                Anagrafica[] myListanagrafiche = null;
                GestioneAreaFamiliariAreaFamiliare[] mylistfamiliari = null;
                esito = proxy.CancelFamiliari(out mylistfamiliari, out myListanagrafiche, numeroDomanda);

                familiari.areaFamiliare = mylistfamiliari.ToList();
                familiari.anagrafica = myListanagrafiche.ToList();

                familiari.HasError = esito.RisultatoOperazione == AreaEsito.TipoEsito.KO ? true : false;
                familiari.ErrorMessage = esito.Messaggio;

                MergeListAnagraficaFamiliare(familiari, titolarePensione);
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterFamiliari, Errore nel metodo CancellaFamiliari");
            }
            finally
            {
                Utility.CloseClient(proxy);
            }
        }

        #region private methods

        private void SeparateListFamiliariFull(IFamiliari familiari)
        {
            familiari.areaFamiliare = new List<GestioneAreaFamiliariAreaFamiliare>();
            familiari.anagrafica = new List<Anagrafica>();

            foreach (FamiliareFull fam in familiari.elencoFamiliari)
            {
                familiari.areaFamiliare.Add(fam.areaFamiliare);
                familiari.anagrafica.Add(fam.anagrafica);
            }
        }

        private void MergeListAnagraficaFamiliare(IFamiliari familiari, ITitolarePensione titolarePensione)
        {
            familiari.elencoFamiliari = new List<PresenterFamiliari.FamiliareFull>();

            foreach (GestioneAreaFamiliariAreaFamiliare fam in familiari.areaFamiliare)
            {
                FamiliareFull famFull = new FamiliareFull();
                famFull.areaFamiliare = fam;
                famFull.anagrafica = familiari.anagrafica.Find(x => x.CodiceFiscale == fam.Familiare.CodiceFiscale);
                familiari.elencoFamiliari.Add(famFull);
            }

            SortListFamiliari(familiari, titolarePensione);
        }

        private void SortListFamiliari(IFamiliari familiari, ITitolarePensione titolarePensione)
        {
            if (familiari.elencoFamiliari != null && familiari.elencoFamiliari.Count > 0)
            {
                List<FamiliareFull> famFullApp = new List<FamiliareFull>();
                foreach (FamiliareFull fam in familiari.elencoFamiliari)
                    famFullApp.Add(fam);

                FamiliareFull famConiuge = famFullApp.Find(x => (x.areaFamiliare.Familiare.SiglaFamiliare.HasValue && x.areaFamiliare.Familiare.SiglaFamiliare == 'C'));
                if (famConiuge != null)
                    famFullApp.Remove(famConiuge);
                // ordinamento per data nascita
                famFullApp.Sort((x, y) => DateTime.Compare(x.anagrafica.DataNascita.Value, y.anagrafica.DataNascita.Value));

                // inserimento primo posto il Coniuge
                if (famConiuge != null)
                    famFullApp.Insert(0, famConiuge);

                FamiliareFull famTitolare = famFullApp.Find(x => (x.areaFamiliare.Familiare.CodiceFiscale == titolarePensione.TitolarePensione.Anagrafica.CodiceFiscale));
                if (famTitolare != null)
                {
                    famFullApp.Remove(famTitolare);
                    famFullApp.Insert(0, famTitolare);
                }

                familiari.elencoFamiliari = famFullApp;
            }
        }

        #endregion private methods

        [Serializable]
        public class FamiliareFull
        {
            public FamiliareFull()
            { }

            private Anagrafica _anagrafica;
            private GestioneAreaFamiliariAreaFamiliare _areaFamiliare;

            public Anagrafica anagrafica { get { return _anagrafica; } set { _anagrafica = value; } }
            public GestioneAreaFamiliariAreaFamiliare areaFamiliare { get { return _areaFamiliare; } set { _areaFamiliare = value; } }
        }

    }
}
