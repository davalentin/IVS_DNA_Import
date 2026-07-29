using System;
using System.Collections.Generic;

using INPS.Pensioni.LiquidazionePensione.Presenter.IView;
using INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione;
using System.Globalization;

namespace INPS.Pensioni.LiquidazionePensione.Presenter
{
    public class PresenterDanteCausa
    {
        public void GetDatiDanteCausa(IDanteCausa DanteCausa)
        {
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                AreaDanteCausa datiDanteCausa;
                AreaEsito esito = new AreaEsito();
                AreaRichiestaDomanda areaRichiestaDomanda = new AreaRichiestaDomanda();
                areaRichiestaDomanda.NumeroDomanda = Int64.Parse(DanteCausa.domanda.NumeroDomanda);
                areaRichiestaDomanda.ProgStorico = DanteCausa.domanda.ProgStorico;

                esito = objWS.GetDanteCausaByDomanda(out datiDanteCausa, areaRichiestaDomanda);
                DanteCausa.areaDanteCausa = datiDanteCausa;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo GetDatiDanteCausa");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDanteCausaByDomanda(IDanteCausa DanteCausa)
        {
            Presenter.SvrLiquidazione.ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                Presenter.SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();

                esito = objWS.StoreDanteCausa(Int64.Parse(DanteCausa.domanda.NumeroDomanda), DanteCausa.areaDanteCausa);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    DanteCausa.HasError = true;
                    DanteCausa.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo SalvaDanteCausaByDomanda");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiAnagraficaDC(IDanteCausa danteCausa)
        {
            string sErrore;
            ServizioLiquidazioneClient objWS = new ServizioLiquidazioneClient();
            try
            {
                AreaEsito esito = new AreaEsito();
                long ndomus = Int64.Parse(danteCausa.domanda.NumeroDomanda);
                esito = objWS.StoreDatiAnagraficaDC(ndomus, danteCausa.areaDanteCausa);

                if (esito.RisultatoOperazione == AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    danteCausa.HasError = true;
                    danteCausa.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo SalvaDatiAnagraficaDC");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiAltraPensione(IDanteCausa danteCausa)
        {
            string sErrore;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(danteCausa.domanda.NumeroDomanda);
                esito = objWS.StoreDatiAltraPensione(ndomus, danteCausa.areaDanteCausa);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    danteCausa.HasError = true;
                    danteCausa.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo SalvaDatiAltraPensione");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiPensioneCI(IDanteCausa danteCausa)
        {
            string sErrore;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(danteCausa.domanda.NumeroDomanda);
                esito = objWS.StoreDatiPensioneCI(ndomus, danteCausa.areaDanteCausa);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    danteCausa.HasError = true;
                    danteCausa.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo SalvaDatiPensioneCI");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiPensioneDiretta(IDanteCausa danteCausa)
        {
            string sErrore;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(danteCausa.domanda.NumeroDomanda);
                esito = objWS.StoreDatiPensioneDiretta(ndomus, danteCausa.areaDanteCausa);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    danteCausa.HasError = true;
                    danteCausa.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo SalvaDatiPensioneDiretta");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void SalvaDatiRedditi(IDanteCausa danteCausa)
        {
            string sErrore;
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(danteCausa.domanda.NumeroDomanda);
                esito = objWS.StoreDatiRedditiSentenza49593(ndomus, danteCausa.areaDanteCausa);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    danteCausa.HasError = true;
                    danteCausa.ErrorMessage = esito.Messaggio;
                }
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo SalvaDatiRedditi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public void EliminaRedditi(IDanteCausa danteCausa)
        {
            string sErrore;
            AreaDanteCausa areaDanteCausa = new AreaDanteCausa();
            SvrLiquidazione.ServizioLiquidazioneClient objWS = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.ServizioLiquidazioneClient();
            try
            {
                SvrLiquidazione.AreaEsito esito = new INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito();
                long ndomus = Int64.Parse(danteCausa.domanda.NumeroDomanda);
                esito = objWS.CancelDanteSentenza495_93(out areaDanteCausa, ndomus);

                if (esito.RisultatoOperazione == INPS.Pensioni.LiquidazionePensione.Presenter.SvrLiquidazione.AreaEsito.TipoEsito.KO)
                {
                    sErrore = esito.Messaggio;
                    danteCausa.HasError = true;
                    danteCausa.ErrorMessage = esito.Messaggio;
                }

                danteCausa.areaDanteCausa = areaDanteCausa;
            }
            catch (Exception ex)
            {
                Utility.ExceptionHandler(ex, "PresenterDanteCausa, Errore nel metodo SalvaDatiRedditi");
            }
            finally
            {
                Utility.CloseClient(objWS);
            }
        }

        public List<KeyValuePair<string, string>> GestioneCategoriePensioni(AreaDecodifica.DatiCategoriaPensione[] lista)
        {
            List<KeyValuePair<string, string>> ListReturn = new List<KeyValuePair<string, string>>();

            foreach (AreaDecodifica.DatiCategoriaPensione categoria in lista)
            {
                int? codice = categoria.Codice.Trim() != string.Empty ? Convert.ToInt32(categoria.Codice.Trim()) : (int?)null;
                if (codice.HasValue && codice < 99 || (codice > 200 && codice < 207) || (codice > 212 && codice < 243) || codice >= 170 && codice <= 172 || codice == 198 || codice == 199 || codice == 801 ||
                    codice == 802 || codice == 243 || codice == 244 || codice == 245) //ENG - Spacchettate SOPGI
                {
                    if (categoria.Tipo == 'C')
                    {
                        string Sigla = string.Empty;
                        string SiglaI = string.Empty;
                        string SiglaV = string.Empty;
                        string SiglaS = string.Empty;
                        string Codice = string.Empty;

                        Codice = categoria.Codice;

                        SiglaI = "I" + categoria.Sigla.Trim();
                        if(codice == 94)
                        {
                            foreach (Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI valore in System.Enum.GetValues(typeof(Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI)))
                            {
                                string val = valore.ToString();
                                if (val == "Uno") val = "1";
                                ListReturn.Add(new KeyValuePair<string, string>(SiglaI + val, SiglaI + val));
                            }
                        }
                        ListReturn.Add(new KeyValuePair<string, string>(SiglaI, SiglaI));

                        SiglaV = "V" + categoria.Sigla.Trim();
                        if (codice == 94)
                        {
                            foreach (Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI valore in System.Enum.GetValues(typeof(Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI)))
                            {
                                string val = valore.ToString();
                                if (val == "Uno") val = "1";
                                ListReturn.Add(new KeyValuePair<string, string>(SiglaV + val, SiglaV + val));
                            }
                        }
                        ListReturn.Add(new KeyValuePair<string, string>(SiglaV, SiglaV));

                        SiglaS = "S" + categoria.Sigla.Trim();
                        if (codice == 94)
                        {
                            foreach (Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI valore in System.Enum.GetValues(typeof(Presenter.SvrLiquidazioneFs.UtilityCategoriaFondoPI)))
                            {
                                string val = valore.ToString();
                                if (val == "Uno") val = "1";
                                ListReturn.Add(new KeyValuePair<string, string>(SiglaS + val, SiglaS + val));
                            }
                        }
                        ListReturn.Add(new KeyValuePair<string, string>(SiglaS, SiglaS));
                    }
                    else
                    {
                        string Sigla = string.Empty;
                        string Codice = string.Empty;
                        Codice = categoria.Codice;
                        Sigla = categoria.Sigla.Trim();
                        ListReturn.Add(new KeyValuePair<string, string>(Sigla, Sigla));
                    }
                }
            }
            ListReturn.Sort((x, y) => string.Compare(x.Key, y.Key, false, CultureInfo.InvariantCulture));
            return ListReturn;
        }

        public List<KeyValuePair<string, string>> GestioneCategoriePensioniAltraPensione(List<AreaDecodifica.DatiCategoriaPensione> lista)
        {
            List<KeyValuePair<string, string>> ListReturn = new List<KeyValuePair<string, string>>();

            foreach (AreaDecodifica.DatiCategoriaPensione categoria in lista)
            {
                if (categoria.Codice.Trim() != string.Empty && Convert.ToInt32(categoria.Codice.Trim()) < 99)
                {
                    if (categoria.Codice.Length > 3)
                        categoria.Codice = categoria.Codice.Substring(1);
                    if (!ListReturn.Contains(new KeyValuePair<string, string>(categoria.Codice, categoria.Codice)))
                        ListReturn.Add(new KeyValuePair<string, string>(categoria.Codice, categoria.Codice));
                }
            }
            ListReturn.Sort((x, y) => string.Compare(x.Key, y.Key, false, CultureInfo.InvariantCulture));
            return ListReturn;
        }

        public List<KeyValuePair<string, string>> GestioneCategoriePensioniAltraPensione(List<AreaDecodifica.DatiCategoriaAltraPensione> lista)
        {
            List<KeyValuePair<string, string>> ListReturn = new List<KeyValuePair<string, string>>();

            foreach (AreaDecodifica.DatiCategoriaAltraPensione categoria in lista)
            {
                if (categoria.CodCategoria.Trim() != string.Empty)
                {
                    int codCat = 0;
                    int.TryParse(categoria.CodCategoria.Trim(), out codCat);
                    if (codCat > 0)
                    {
                        if (codCat < 99 || (codCat >= 170 && codCat <= 172) || (codCat >= 201 && codCat <= 242) || codCat == 701 || codCat == 704)
                        {
                            string strCodCat = codCat.ToString().PadLeft(3, '0');
                            if (!ListReturn.Contains(new KeyValuePair<string, string>(strCodCat, strCodCat)))
                                ListReturn.Add(new KeyValuePair<string, string>(strCodCat, strCodCat));
                        }
                    }
                    else
                    {
                        if (!ListReturn.Contains(new KeyValuePair<string, string>(categoria.CodCategoria.Trim(), categoria.CodCategoria.Trim())))
                            ListReturn.Add(new KeyValuePair<string, string>(categoria.CodCategoria.Trim(), categoria.CodCategoria.Trim()));
                    }
                }
            }
            ListReturn.Sort((x, y) => string.Compare(x.Key, y.Key, false, CultureInfo.InvariantCulture));
            return ListReturn;
        }
    }
}

