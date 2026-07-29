using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Transactions;
using INPS.DNA.Data;

namespace INPS.Pensioni.Liquidazione
{
    public class GestioneAreaPeriodiAventiDiritto
    {
        #region public methods
        /// <summary>
        /// Recupera le informazioni dell'area da restituire alla View
        /// </summary>
        /// <param name="datiPensione"></param>
        /// <param name="areaPeriodi"></param>
        /// <param name="messaggioVideo"></param>
        /// <returns></returns>
        public static bool GetAreaPeriodiAventiDirittoByDatiPensione(GestionePensione.DatiPensione datiPensione, out Entity.PeriodoAventiDiritto areaPeriodi, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;
            areaPeriodi = new Entity.PeriodoAventiDiritto();

            try
            {
                // GET Familiare
                List<GestioneFamiliari.Familiare> listaDatiFamiliare = null;
                List<GestioneAnagrafica.DatiAnagrafici> listaAnagraficheFamiliari = null;
                GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listaDatiFamiliare, out listaAnagraficheFamiliari);

                GestioneFamiliari.Familiare datiFamiliare = listaDatiFamiliare.Find(x => x.TipoComponente == 'T');
                areaPeriodi.DatiFamiliareAventeDiritto = datiFamiliare;
                areaPeriodi.DatiAnagraficiAventeDiritto = listaAnagraficheFamiliari.Find(x => x.Id == datiFamiliare.IdAnagrafica);

                // GET PeriodiAventiDiritto
                List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
                GestioneAventiDiritto.GetAventiDirittoByIdPensione(datiPensione.Id, out listaAventiDiritto);
                // TODO: Nel caso in cui ci siano due record su AventiDiritto con stesso IdAnagrafica cosa bisogna fare???
                GestioneAventiDiritto.AventiDiritto aventeDiritto = listaAventiDiritto.Find(x => x.IdAnagrafica == datiFamiliare.IdAnagrafica);
                if (aventeDiritto != null)
                {
                    List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodi = null;
                    GestionePeriodiAventiDiritto.GetPeriodiAventiDiritto(datiPensione.Id, aventeDiritto.Id, out listaPeriodi);
                    areaPeriodi.ListaPeriodiAventeDiritto = listaPeriodi;
                    areaPeriodi.IdAventeDiritto = aventeDiritto.Id;
                }
            }
            catch (Exception Ex)
            {
                messaggioVideo = Ex.Message;
                INPS.DNA.Logging.Logger.LogException(Ex);
                return false;
            }

            return true;
        }

        public static bool ControlsDatiPeriodiAventiDiritto(GestionePensione.DatiPensione datiPensione, List<GestionePeriodiAventiDiritto.PeriodoAventiDiritto> listaPeriodiAventeDiritto,
            GestioneAnagrafica.DatiAnagrafici datiAnagraficiAventeDiritto, GestioneAventiDiritto.AventiDiritto aventeDiritto, GestioneAnagrafica.DatiAnagrafici anagraficaDanteCausa,
            bool isAventeDirittoTitolareIncongruente, bool isRiaperturaDomanda, BLCommon.GestioneDanteCausa.DatiDanteCausa danteCausa, out string messaggioVideo)
        {
            messaggioVideo = string.Empty;

            if (isAventeDirittoTitolareIncongruente)
            {
                messaggioVideo = "E' presente un'incongruenza nell'Avente Diritto Titolare. Risolvere prima l'incongruenza.";
                return false;
            }

            if (listaPeriodiAventeDiritto == null || listaPeriodiAventeDiritto.Count == 0)
            {
                messaggioVideo = "E' obbligatorio inserire almeno un periodo.";
                return false;
            }
            else
            {
                foreach (GestionePeriodiAventiDiritto.PeriodoAventiDiritto periodo in listaPeriodiAventeDiritto)
                {
                    if (!periodo.GradoParentela.HasValue)
                    {
                        messaggioVideo = "Il Grado di Parentela è obbligatorio.";
                        return false;
                    }

                    if (!periodo.DecorrenzaPeriodo.HasValue)
                    {
                        messaggioVideo = "La Decorrenza Periodo è obbligatoria.";
                        return false;
                    }

                    if (listaPeriodiAventeDiritto.Count(x => x.Equals(periodo)) > 1)
                    {
                        messaggioVideo = "Non è possibile inserire più periodi uguali.";
                        return false;
                    }

                    if (datiAnagraficiAventeDiritto != null && datiAnagraficiAventeDiritto.DataNascita.HasValue && danteCausa != null && danteCausa.DataMorte.HasValue && periodo.GradoParentela.HasValue && periodo.GradoParentela.Value == 'O' &&
                        Utility.DataSuccessivaA(datiAnagraficiAventeDiritto.DataNascita.Value.AddYears(18), danteCausa.DataMorte.Value))
                    {
                        messaggioVideo = "Il familiare deve avere compiuto 18 anni alla data morte del dante causa";
                        return false;
                    }
                }

                if (!GestioneCrossControls.AGO_ControlsGradoParentelaPeriodi(listaPeriodiAventeDiritto, datiAnagraficiAventeDiritto.DataNascita.Value, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.ALL_ControlsGradoParentelaPercGiudicePeriodi(datiPensione, listaPeriodiAventeDiritto, datiAnagraficiAventeDiritto.DataNascita.Value, danteCausa , out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.AGO_ControlsDecorrenzaPeriodi(datiPensione, aventeDiritto, listaPeriodiAventeDiritto, isRiaperturaDomanda, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.AGO_ControlsCessazionePeriodoAventiDiritto(listaPeriodiAventeDiritto, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.AGO_ControlsSovrapposizionePeriodiAventiDiritto(listaPeriodiAventeDiritto, out messaggioVideo))
                    return false;

                if (!GestioneCrossControls.AGO_ControlsSessoPeriodiAventiDiritto(listaPeriodiAventeDiritto, datiAnagraficiAventeDiritto, anagraficaDanteCausa != null ? anagraficaDanteCausa.Sesso : null,
                    out messaggioVideo))
                    return false;
            }

            return true;
        }

        public static bool StoreDatiPeriodi(GestionePensione.DatiPensione datiPensione, Entity.PeriodoAventiDiritto areaPeriodi)
        {
            if (areaPeriodi == null || areaPeriodi.Equals(new Entity.PeriodoAventiDiritto()))
                return false;

            GestioneQuadri.DatiQuadroPeriodi datiQuadroPeriodi = null;
            GestioneQuadri.GetQuadroPeriodiByDatiPensione(datiPensione, out datiQuadroPeriodi);

            List<GestioneAventiDiritto.AventiDiritto> listaAventiDiritto = null;
            GestioneAventiDiritto.GetAventiDirittoByIdPensione(datiPensione.Id, out listaAventiDiritto);
            GestioneAventiDiritto.AventiDiritto datiAventeDiritto = (listaAventiDiritto != null && listaAventiDiritto.Count > 0) ? listaAventiDiritto.FirstOrDefault(x => x.Id == areaPeriodi.IdAventeDiritto) : null;

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneFamiliari.SalvaFamiliare(areaPeriodi.DatiFamiliareAventeDiritto, null, areaPeriodi.DatiAnagraficiAventeDiritto, null, datiPensione.Id, datiPensione.SiglaCategoria);

                areaPeriodi.ListaPeriodiAventeDiritto.ForEach(x => x.IdAventeDiritto = areaPeriodi.IdAventeDiritto);
                GestionePeriodiAventiDiritto.SavePeriodiAventiDiritto(datiPensione.Id, areaPeriodi.ListaPeriodiAventeDiritto);

                if (datiAventeDiritto != null)
                {
                    datiAventeDiritto.DecParentelaDA = areaPeriodi.ListaPeriodiAventeDiritto.OrderBy(x => x.DecorrenzaPeriodo).First().GradoParentela;
                    GestioneAventiDiritto.SalvaAventeDiritto(datiAventeDiritto);
                }

                datiQuadroPeriodi.TabPeriodi = 2;
                GestioneQuadri.SalvaQuadroPeriodi(datiPensione.Id, datiQuadroPeriodi);

                transactionScope.Complete();
            }

            return true;
        }

        public static void DeleteDatiPeriodi(GestionePensione.DatiPensione datiPensione, Entity.PeriodoAventiDiritto areaPeriodi)
        {
            GestioneQuadri.DatiQuadroPeriodi datiQuadroPeriodi = null;
            GestioneQuadri.GetQuadroPeriodiByDatiPensione(datiPensione, out datiQuadroPeriodi);

            using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                GestioneFamiliari.SalvaFamiliare(areaPeriodi.DatiFamiliareAventeDiritto, null, areaPeriodi.DatiAnagraficiAventeDiritto, null, datiPensione.Id, datiPensione.SiglaCategoria);
                GestionePeriodiAventiDiritto.DeletePeriodiAventiDirittoByIdAventeDiritto(areaPeriodi.IdAventeDiritto);

                if (areaPeriodi.ListaPeriodiAventeDiritto != null && areaPeriodi.ListaPeriodiAventeDiritto.Count > 0)
                    GestionePeriodiAventiDiritto.SavePeriodiAventiDiritto(datiPensione.Id, areaPeriodi.ListaPeriodiAventeDiritto);

                datiQuadroPeriodi.TabPeriodi = 0;

                GestioneQuadri.SalvaQuadroPeriodi(datiPensione.Id, datiQuadroPeriodi);

                transactionScope.Complete();
            }
        }

        public static void GetDecodificaGradoParentela(GestionePensione.DatiPensione datiPensione, out List<GestioneAreaFamiliari.AreaDecFam.DatiSiglaFamiliare> elencoGradoParentela)
        {
            elencoGradoParentela = null;

            #region Get TipoAppartenenza

            Utility.TipoAppartenenza? tipoAppartenenza = Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione);
            string tipologia = string.Empty;
            if (tipoAppartenenza.HasValue)
            {
                switch (tipoAppartenenza.Value)
                {
                    case Utility.TipoAppartenenza.FS:
                        tipologia = "FS";
                        break;
                    case Utility.TipoAppartenenza.AGO:
                        tipologia = "AGO";
                        break;
                    case Utility.TipoAppartenenza.CI:
                        tipologia = "CI";
                        break;
                }
            }
            #endregion Get TipoAppartenenza

            List<BLCommon.GestioneDecodifica.SiglaFamiliare> elencoGradiParentelaBL = null;
            BLCommon.GestioneDecodifica.GetSiglaFamiliareByTipologia(tipologia, out elencoGradiParentelaBL);
            FiltraGradiParentela(tipoAppartenenza, datiPensione, ref elencoGradiParentelaBL);

            if (elencoGradiParentelaBL != null && elencoGradiParentelaBL.Count > 0)
                elencoGradoParentela = elencoGradiParentelaBL.OrderBy(x => x.Descrizione).Select(x => new GestioneAreaFamiliari.AreaDecFam.DatiSiglaFamiliare(x)).ToList();
        }
        #endregion public methods

        #region private methods
        private static void FiltraGradiParentela(Utility.TipoAppartenenza? tipoAppartenenza, GestionePensione.DatiPensione datiPensione,
            ref List<BLCommon.GestioneDecodifica.SiglaFamiliare> elencoSiglaFamiliareBL)
        {
            if (tipoAppartenenza.HasValue && elencoSiglaFamiliareBL != null && elencoSiglaFamiliareBL.Count > 0)
            {
                switch (tipoAppartenenza.Value)
                {
                    case Utility.TipoAppartenenza.AGO:
                        string codCat = datiPensione.GetCodCategoria();
                        codCat = codCat.PadLeft(4, '0');


                        for (int i = 0; i < elencoSiglaFamiliareBL.Count; i++)
                        {
                            bool remove = false;
                            BLCommon.GestioneDecodifica.SiglaFamiliare s = elencoSiglaFamiliareBL[i];
                            if (((s.Id == "Z" || s.Id == "K" || s.Id == "W") && !Utility.IsDomandaAGOReversibile(datiPensione)) ||
                                ((s.Id == "G" || s.Id == "P" || s.Id == "Y") && Utility.IsDomandaAGOReversibile(datiPensione)) ||
                                ((s.Id == "X" || s.Id == "B" || s.Id == "D") && codCat != "0072"))
                                remove = true;
                            else if (s.Id == "N")
                            {
                                if (codCat == "0037" || codCat == "0040")
                                {
                                    try
                                    {
                                        s.Descrizione = s.Descrizione.Substring(s.Descrizione.IndexOf('/') + 1);
                                    }
                                    catch (Exception)
                                    {
                                        // Eccezione ignorata
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        s.Descrizione = s.Descrizione.Substring(0, s.Descrizione.IndexOf('/'));
                                    }
                                    catch (Exception)
                                    {
                                        // Eccezione ignoarata
                                    }
                                }
                            }

                            if (remove)
                            {
                                elencoSiglaFamiliareBL.RemoveAt(i);
                                i--;
                            }
                        }
                        break;
                    case Utility.TipoAppartenenza.FS:
                        List<BLCommon.GestioneDecodifica.SiglaFamiliare> app = elencoSiglaFamiliareBL.ToList();
                        foreach (BLCommon.GestioneDecodifica.SiglaFamiliare siglaFamiliareBL in app)
                        {
                            switch (siglaFamiliareBL.Id)
                            {
                                case "K":
                                case "W":
                                case "Z":
                                    if (Utility.GetTipoDomanda(datiPensione.Gruppo, datiPensione.Prodotto) != Utility.TipoDomanda.Superstiti)
                                        elencoSiglaFamiliareBL.Remove(siglaFamiliareBL);
                                    break;
                                default:
                                    // DO NOTHING
                                    break;
                            }
                        }
                        break;
                }
            }
        }
        #endregion private methods
    }
}
