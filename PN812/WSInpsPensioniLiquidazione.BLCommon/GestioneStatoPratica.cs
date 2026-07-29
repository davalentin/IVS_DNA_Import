using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.Pensioni.Liquidazione.DataCommon;
using System.IO;
using System.Linq.Expressions;


namespace INPS.Pensioni.Liquidazione.BLCommon
{
    public class GestioneStatoPratica
    {
        #region public members

        public static void GetPensioneByNumeroDomanda(long numeroDomanda, string matricola, int sedeDiAppartenenzaOperatore, out List<DatiDomandaDettagliata> elencoDatiDomandaDettagliata)
        {
            // la 'lista risultati ricerca' pari a null vuol dire che nessuna ricerca è stata ancora effettuata
            List<DAGestioneStatoPratica.RisultatoRicerca> listRisultatiRicerca = null;
            elencoDatiDomandaDettagliata = new List<DatiDomandaDettagliata>();

            // ricerca per NUMERO DOMANDA
            DAGestioneStatoPratica.GetPensioniByNumeroDomanda(numeroDomanda, ref listRisultatiRicerca);

            List<GestioneDecodifica.Prodotto> elencoProdotto = null;
            GestioneDecodifica.GetProdotto(out elencoProdotto);

            List<GestioneDecodifica.Tipo> elencoTipo = null;
            GestioneDecodifica.GetTipo(out elencoTipo);

            List<GestioneDecodifica.DecSede> elencoDecSede = null;
            GestioneDecodifica.GetElencoDecSede(out elencoDecSede);


            foreach (DAGestioneStatoPratica.RisultatoRicerca risultatoRicerca in listRisultatiRicerca)
            {
                //// Recupero la descrizione dellla tipologia di domanda
                string descProdotto = elencoProdotto.Find(x => x.CodProdotto == risultatoRicerca.Prodotto).DescProdotto;
                string descTipo = elencoTipo.Find(x => x.CodTipo == risultatoRicerca.Tipo).DescTipo;

                if (Utility.IsPensioniOvunqueAttiva(Utility.GetTipoAppartenenza(risultatoRicerca.IndConvInt, risultatoRicerca.Gestione)) && (Utility.IsRicostituzione(risultatoRicerca.Gruppo) || Utility.IsRiaperturaDomanda(risultatoRicerca.CodFase)) && risultatoRicerca.CodiceSedeGP1ALZ6.HasValue
                    && !risultatoRicerca.SedeDestinazione.HasValue)
                {
                    risultatoRicerca.Sede = risultatoRicerca.CodiceSedeGP1ALZ6.GetValueOrDefault();
                    //ENG - Implementazione Meta Processo        
                    GestioneControlliDinamici.ControlloDinamico ctrlMetaProcesso = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrlMetaProcesso);
                    if (ctrlMetaProcesso != null && !String.IsNullOrEmpty(ctrlMetaProcesso.ValoreControllo) && ctrlMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                    {
                        if (risultatoRicerca.CodiceSedeLavorazione.HasValue && risultatoRicerca.CodiceSedeLavorazione.Value != risultatoRicerca.CodiceSedeGP1ALZ6.Value)
                            risultatoRicerca.Sede = risultatoRicerca.CodiceSedeLavorazione.Value;
                    }
                    //risultatoRicerca.CentroOperativo = risultatoRicerca.CentroOperativoGP1ALZ6.GetValueOrDefault();
                }

                //31/01/2022: verifico se la sede della domanda è chiusa e si trova nella stessa provincia della sede di appartenenza dell'operatore
                GestioneDecodifica.DecSede decSedeChiusa = null;
                bool isSedeChiusaStessaProvinciaOperatore = false;
                if (elencoDecSede != null && elencoDecSede.Count > 0)
                {
                    decSedeChiusa = elencoDecSede.FindAll(x => !String.IsNullOrEmpty(x.CodProvincia) && risultatoRicerca.Sede.ToString().PadLeft(4, '0').Substring(0, 2) == x.CodProvincia.PadLeft(3, '0').Substring(1, 2)
                         && !String.IsNullOrEmpty(x.CodZona) && risultatoRicerca.Sede.ToString().PadLeft(4, '0').Substring(2, 2) == x.CodZona.PadLeft(3, '0').Substring(1, 2)
                         && !String.IsNullOrEmpty(x.CodCentroOperativo) && risultatoRicerca.CentroOperativo.ToString().PadLeft(2, '0').Substring(0, 2) == x.CodCentroOperativo.PadLeft(3, '0').Substring(1, 2)
                         && x.CodAttivitaSede.GetValueOrDefault() == '0').FirstOrDefault();
                    isSedeChiusaStessaProvinciaOperatore = (decSedeChiusa != null && !String.IsNullOrEmpty(decSedeChiusa.CodProvincia)) ? decSedeChiusa.CodProvincia.PadLeft(3, '0').Substring(1, 2) == sedeDiAppartenenzaOperatore.ToString().PadLeft(6, '0').Substring(0, 2) : false;
                }

                if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(risultatoRicerca.Gruppo, risultatoRicerca.Prodotto, risultatoRicerca.Tipo))
                    descProdotto = "Adeguamento Pro Quota";

                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(risultatoRicerca.Gruppo, risultatoRicerca.Prodotto);
                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                    risultatoRicerca.Tipo = "RIC";
                else if (Utility.IsRiaperturaDomanda(risultatoRicerca.CodFase))
                    risultatoRicerca.Tipo = "TRF";
                else
                    risultatoRicerca.Tipo = "PL";

                //18-07-2013: inserito controllo per restituire solo domande con matricola pari a quella dell'utente (non vale per amministratore o direttore_RdP)
                //31-01-2022: inserito controllo per restituire anche le domande che hanno la sede chiusa ma codProvincia uguale a quella della sede di appartenenza dell'operatore
                //21-12-2023: le domande automatizate vengono sempre restituite indipendentemente dal tipo di utenza
                if ((!string.IsNullOrEmpty(matricola)) && risultatoRicerca.TipoAutomazione == null && risultatoRicerca.MatricolaUtenteAcquisizione != matricola && !isSedeChiusaStessaProvinciaOperatore)
                    continue;

                DatiDomandaDettagliata datiDomandaDettagliata = new DatiDomandaDettagliata(risultatoRicerca.Nome, risultatoRicerca.Cognome, risultatoRicerca.CodiceFiscale,
                    risultatoRicerca.Sede.ToString().PadLeft(4, '0'), risultatoRicerca.CentroOperativo.ToString().PadLeft(2, '0'),
                    risultatoRicerca.SedeDestinazione.HasValue ? risultatoRicerca.SedeDestinazione.ToString().PadLeft(4, '0') : null,
                    risultatoRicerca.SedeDestinazione.HasValue ? (risultatoRicerca.CentroOperativoDestinazione.HasValue ? risultatoRicerca.CentroOperativoDestinazione.ToString().PadLeft(2, '0') : "00") : null,
                    risultatoRicerca.Categoria, risultatoRicerca.Tipo, risultatoRicerca.Fondo, risultatoRicerca.Stato,
                    risultatoRicerca.NumeroDomanda.ToString().PadLeft(13, '0'), risultatoRicerca.ProgStorico, risultatoRicerca.Certificato.ToString().PadLeft(8, '0'),
                    risultatoRicerca.DataPresentazioneDomanda, risultatoRicerca.DataElaborazioneDomanda, risultatoRicerca.IndConvInt,
                    risultatoRicerca.Gestione, risultatoRicerca.MatricolaUtenteAcquisizione, descProdotto, descTipo);
                elencoDatiDomandaDettagliata.Add(datiDomandaDettagliata);
            }
        }

        public static void AggiornaInfoPratica(GestionePensione.DatiPensione datiPensione, bool IsCalcoloAbilitato, out bool IsGestioneAttivita, out byte? statoPensione, out string matricolaUtenteAcquisizione)
        {
            statoPensione = null;
            matricolaUtenteAcquisizione = string.Empty;
            IsGestioneAttivita = false;

            if (datiPensione != null)
            {
                if (IsCalcoloAbilitato && datiPensione.StatoPensione.HasValue && datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.InAcquisizione)
                {
                    //DA STATO 1 (IN ACQUISIZIONE) PASSA ALLO STATO 3 (DA CALCOLARE)
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.DaCalcolare;
                    BLCommon.GestionePensione.SalvaPensione(datiPensione);
                    IsGestioneAttivita = true;
                }
                else if (!IsCalcoloAbilitato && datiPensione.StatoPensione.HasValue &&
                    (datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.DaCalcolare ||
                    datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.CalcoloVerify ||
                    datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.ScartoDaCalcolo ||
                    datiPensione.StatoPensione.Value == (int)Utility.StatoPensione.ScartoVerify))
                {
                    //PASSA ALLO STATO 1 (IN ACQUISIZIONE)
                    datiPensione.StatoPensione = (int)Utility.StatoPensione.InAcquisizione;
                    BLCommon.GestionePensione.SalvaPensione(datiPensione);
                    IsGestioneAttivita = true;
                }
                statoPensione = datiPensione.StatoPensione;
                matricolaUtenteAcquisizione = datiPensione.MatricolaUtenteAcquisizione;
            }
        }
        #endregion public members


        /// <summary>
        /// Ottimizzazione del metodo GetPensioniByCriteriMultipli() viene creata la clausola della where utilizzata per il recupero dei dati
        /// in maniera dinamica in base ai filtri che sono stati settati.
        /// </summary>
        public static void GetPensioniByCriteriMultipliOptimized(string nome, string cognome, string codiceFiscale, string sede, string categoria, string tipo, string fondo,
             short? statoPensione, int? certificato, DateTime dataPresentazioneDomandaMin, DateTime dataPresentazioneDomandaMax,
             DateTime dataElaborazioneMin, DateTime dataElaborazioneMax, string matricola, Utility.TipoAppartenenza tipoAppOperatore, Utility.Ruolo ruolo, TipoDomanda tipoDomandaInLavorazione,
             TipoDomanda tipoDomandaLavorata, string gruppo, string prodotto, string cassa, out List<DatiDomandaDettagliata> elencoDatiDomandaDettagliata)
        {
            // la lista risultati ricerca' pari a null vuol dire che nessuna ricerca è stata ancora effettuata
            List<DAGestioneStatoPratica.RisultatoRicerca> listRisultatiRicerca = null;
            elencoDatiDomandaDettagliata = new List<DatiDomandaDettagliata>();

            // espressione delle clausola where
            Expression<Func<Pensione, bool>> whereCondition = (p) => true;

            // ricerca per NUMERO CERTIFICATO
            if (certificato != null)
            {
                Expression<Func<Pensione, bool>> predicateNumCertificato = p => p.NCertificato == certificato;
                whereCondition = whereCondition.And(predicateNumCertificato);
            }
            // ricerca per CODICE FISCALE
            if (!String.IsNullOrEmpty(codiceFiscale))
            {
                long idAnagrafica = 0;
                GestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(codiceFiscale, out idAnagrafica);

                Expression<Func<Pensione, bool>> predicateAnagrafica = p => p.Titolares.Any(x => x.IdAnagrafica == idAnagrafica);
                whereCondition = whereCondition.And(predicateAnagrafica);
            }
            // ricerca per NOME E COGNOME
            if (!String.IsNullOrEmpty(nome) && !String.IsNullOrEmpty(cognome))
            {
                List<long> listaIdAnagrafica = null;
                GestioneAnagrafica.GetIdAnagraficaByNomeCognome(nome, cognome, out listaIdAnagrafica);

                Expression<Func<Pensione, bool>> predicateNomeCognome = p => p.Titolares.Any(x => listaIdAnagrafica.Contains(x.IdAnagrafica));
                whereCondition = whereCondition.And(predicateNomeCognome);
            }
            // ricerca per DATA PRESENTAZIONE DOMANDA
            if (dataPresentazioneDomandaMin != DateTime.MinValue && dataPresentazioneDomandaMax != DateTime.MinValue)
            {
                Expression<Func<Pensione, bool>> predicateDataPresDomanda = p => p.DataPresentazioneDomanda >= dataPresentazioneDomandaMin
                                                && p.DataPresentazioneDomanda <= dataPresentazioneDomandaMax;
                whereCondition = whereCondition.And(predicateDataPresDomanda);
            }
            // ricerca per DATA ELABORAZIONE DOMANDA
            if (dataElaborazioneMin != DateTime.MinValue && dataElaborazioneMax != DateTime.MinValue)
            {
                Expression<Func<Pensione, bool>> predicateDataElabDomanda = p => p.DataElaborazione >= dataElaborazioneMin
                                                && p.DataElaborazione <= dataElaborazioneMax;
                whereCondition = whereCondition.And(predicateDataElabDomanda);
            }
            // ricerca per MATRICOLA
            if (!String.IsNullOrEmpty(matricola))
            {
                Expression<Func<Pensione, bool>> predicatoMatricola = p => p.MatricolaUtenteAcquisizione == matricola;
                whereCondition = whereCondition.And(predicatoMatricola);
            }
            // ricerca per CATEGORIA
            if (!String.IsNullOrEmpty(categoria))
            {
                Expression<Func<Pensione, bool>> pensioneCategoria = p => p.SiglaCategoria == categoria;
                whereCondition = whereCondition.And(pensioneCategoria);
            }
            // ricerca per FONDO
            if (!String.IsNullOrEmpty(fondo))
            {
                Expression<Func<Pensione, bool>> predicateFondo = p => p.Gestione == "007" && p.Fondo == fondo;
                whereCondition = whereCondition.And(predicateFondo);
            }
            // ricerca per CASSA
            if (!String.IsNullOrEmpty(cassa))
            {
                Expression<Func<Pensione, bool>> predicateFondo = p => p.Gestione == "019" && p.Fondo == cassa;
                whereCondition = whereCondition.And(predicateFondo);
            }
            // ricerca per SEDE
            if (!String.IsNullOrEmpty(sede))
            {
                sede = sede.PadRight(6, '0');
                short sedeDomanda = 0;
                short centroOperativoDomanda = 0;
                short.TryParse(sede.Substring(0, 4), out sedeDomanda);
                short.TryParse(sede.Substring(4, 2), out centroOperativoDomanda);
                Expression<Func<Pensione, bool>> predicateSede = p => (p.CodiceSede == sedeDomanda && p.CentroOperativo == centroOperativoDomanda && (!p.CodiceSedeGP1ALZ6.HasValue || p.CodiceSedeDestinazione.HasValue))
                    || (p.CodiceSedeGP1ALZ6.HasValue && ((!p.CodiceSedeLavorazione.HasValue && p.CodiceSedeGP1ALZ6 == sedeDomanda) || (p.CodiceSedeLavorazione.HasValue && p.CodiceSedeLavorazione == sedeDomanda)) && p.CentroOperativo == centroOperativoDomanda);
                whereCondition = whereCondition.And(predicateSede);

            }
            //ricerca per TIPO APPARTENENZA
            if (!String.IsNullOrEmpty(tipoAppOperatore.ToString()))
            {
                switch (tipoAppOperatore.ToString())
                {
                    case "CI":
                        whereCondition = whereCondition.And(p => p.IndConvInt == true && p.Gestione != "018");
                        break;
                    case "AGO":
                        whereCondition = whereCondition.And(p => (p.IndConvInt != true || p.Gestione == "018") && !(p.Gestione == "007" || p.Gestione == "019"));
                        break;
                    case "FS":
                        whereCondition = whereCondition.And(p => p.IndConvInt != true && (p.Gestione == "007" || p.Gestione == "019"));
                        break;
                }
            }
            // ricerca per STATO PENSIONE
            if (statoPensione != null)
            {
                Expression<Func<Pensione, bool>> predicateStatoPensione = p => p.StatoPensione == statoPensione;
                whereCondition = whereCondition.And(predicateStatoPensione);
            }

            // ricerca per PL e/o RIC in lavorazione
            if (tipoDomandaInLavorazione != TipoDomanda.Nessuno)
            {
                Expression<Func<Pensione, bool>> predicateTipoDomandaInLavorazione = p => (p.StatoPensione == 1 || p.StatoPensione == 3 || p.StatoPensione == 5 || p.StatoPensione == 6 || p.StatoPensione == 7);
                switch (tipoDomandaInLavorazione)
                {
                    case TipoDomanda.PL:
                        predicateTipoDomandaInLavorazione = predicateTipoDomandaInLavorazione.And(p => p.Gruppo != "0031");
                        break;
                    case TipoDomanda.RIC:
                        predicateTipoDomandaInLavorazione = predicateTipoDomandaInLavorazione.And(p => p.Gruppo == "0031");
                        break;
                }
                whereCondition = whereCondition.And(predicateTipoDomandaInLavorazione);
            }

            // ricerca per PL e/o RIC Lavorata
            if (tipoDomandaLavorata != TipoDomanda.Nessuno)
            {
                Expression<Func<Pensione, bool>> predicateTipoDomandaLavorata = p => (p.StatoPensione == 4 || p.StatoPensione > 7);
                switch (tipoDomandaLavorata)
                {
                    case TipoDomanda.PL:
                        predicateTipoDomandaLavorata = predicateTipoDomandaLavorata.And(p => p.Gruppo != "0031");
                        break;
                    case TipoDomanda.RIC:
                        predicateTipoDomandaLavorata = predicateTipoDomandaLavorata.And(p => p.Gruppo == "0031");
                        break;
                }
                whereCondition = whereCondition.And(predicateTipoDomandaLavorata);
            }
            // ricerca per GRUPPO
            if (!string.IsNullOrEmpty(gruppo))
            {
                Expression<Func<Pensione, bool>> predicateGruppo = p => p.Gruppo == gruppo;
                whereCondition = whereCondition.And(predicateGruppo);
            }
            // ricerca per PRODOTTO
            if (!string.IsNullOrEmpty(prodotto))
            {
                Expression<Func<Pensione, bool>> predicateProdotto = p => p.Prodotto == prodotto;
                whereCondition = whereCondition.And(predicateProdotto);
            }
            // ricerca per TIPO
            if (!String.IsNullOrEmpty(tipo))
            {
                Expression<Func<Pensione, bool>> predicateTipo = p => p.Tipo == tipo;
                whereCondition = whereCondition.And(predicateTipo);
            }
            //recupero la lista delle pensione che rispettano i criteri di ricerca
            DAGestioneStatoPratica.GetPensioniByExpression(whereCondition, ref listRisultatiRicerca);

            //recupero da NDOM le decodifiche del prodotto e del tipo
            List<GestioneDecodifica.Prodotto> elencoProdotto = null;
            GestioneDecodifica.GetProdotto(out elencoProdotto);
            List<GestioneDecodifica.Tipo> elencoTipo = null;
            GestioneDecodifica.GetTipo(out elencoTipo);

            foreach (DAGestioneStatoPratica.RisultatoRicerca risultatoRicerca in listRisultatiRicerca)
            {
                //// Recupero la descrizione dellla tipologia di domanda
                string descProdotto = elencoProdotto.Find(x => x.CodProdotto == risultatoRicerca.Prodotto).DescProdotto;
                string descTipo = elencoTipo.Find(x => x.CodTipo == risultatoRicerca.Tipo).DescTipo;

                if (Utility.IsPensioniOvunqueAttiva(Utility.GetTipoAppartenenza(risultatoRicerca.IndConvInt, risultatoRicerca.Gestione)) && (Utility.IsRicostituzione(risultatoRicerca.Gruppo) || Utility.IsRiaperturaDomanda(risultatoRicerca.CodFase)) && risultatoRicerca.CodiceSedeGP1ALZ6.HasValue
                     && !risultatoRicerca.SedeDestinazione.HasValue)
                {
                    risultatoRicerca.Sede = risultatoRicerca.CodiceSedeGP1ALZ6.GetValueOrDefault();
                    //ENG - Implementazione Meta Processo        
                    GestioneControlliDinamici.ControlloDinamico ctrlMetaProcesso = null;
                    GestioneControlliDinamici.GetControlloDinamicoByNomeControllo("SbloccaMetaProcesso", out ctrlMetaProcesso);
                    if (ctrlMetaProcesso != null && !String.IsNullOrEmpty(ctrlMetaProcesso.ValoreControllo) && ctrlMetaProcesso.ValoreControllo.Trim().ToUpperInvariant() == "SI")
                    {
                        if (risultatoRicerca.CodiceSedeLavorazione.HasValue && risultatoRicerca.CodiceSedeLavorazione.Value != risultatoRicerca.CodiceSedeGP1ALZ6.Value)
                            risultatoRicerca.Sede = risultatoRicerca.CodiceSedeLavorazione.Value;
                    }

                    //risultatoRicerca.CentroOperativo = risultatoRicerca.CentroOperativoGP1ALZ6.GetValueOrDefault();
                }

                if (Utility.IsDomandaRicostituzioneAdeguamentoProQuotaCasse(risultatoRicerca.Gruppo, risultatoRicerca.Prodotto, risultatoRicerca.Tipo))
                    descProdotto = "Adeguamento Pro Quota";

                Utility.TipoDomanda tipoDomanda = Utility.GetTipoDomanda(risultatoRicerca.Gruppo, risultatoRicerca.Prodotto);
                if (tipoDomanda == Utility.TipoDomanda.Ricostituzione)
                    risultatoRicerca.Tipo = "RIC";
                else if (Utility.IsRiaperturaDomanda(risultatoRicerca.CodFase))
                    risultatoRicerca.Tipo = "TRF";
                else
                    risultatoRicerca.Tipo = "PL";

                DatiDomandaDettagliata datiDomandaDettagliata = new DatiDomandaDettagliata(risultatoRicerca.Nome, risultatoRicerca.Cognome, risultatoRicerca.CodiceFiscale,
                    risultatoRicerca.Sede.ToString().PadLeft(4, '0'), risultatoRicerca.CentroOperativo.ToString().PadLeft(2, '0'),
                    risultatoRicerca.SedeDestinazione.HasValue ? risultatoRicerca.SedeDestinazione.ToString().PadLeft(4, '0') : null,
                    risultatoRicerca.SedeDestinazione.HasValue ? (risultatoRicerca.CentroOperativoDestinazione.HasValue ? risultatoRicerca.CentroOperativoDestinazione.ToString().PadLeft(2, '0') : "00") : null,
                    risultatoRicerca.Categoria,
                    risultatoRicerca.Tipo, risultatoRicerca.Fondo, risultatoRicerca.Stato, risultatoRicerca.NumeroDomanda.ToString().PadLeft(13, '0'), risultatoRicerca.ProgStorico,
                    risultatoRicerca.Certificato.ToString().PadLeft(8, '0'), risultatoRicerca.DataPresentazioneDomanda, risultatoRicerca.DataElaborazioneDomanda,
                    risultatoRicerca.IndConvInt, risultatoRicerca.Gestione, risultatoRicerca.MatricolaUtenteAcquisizione, descProdotto, descTipo);
                elencoDatiDomandaDettagliata.Add(datiDomandaDettagliata);
            }
        }

        #region nested class
        public class DatiDomandaDettagliata
        {
            internal DatiDomandaDettagliata() { }

            public DatiDomandaDettagliata(string nome, string cognome, string codiceFiscale, string sede, string centroOperativo, string sedeDestinazione, string centroOperativoDestinazione, string categoria, string tipo,
                string fondo, string decStatoPensione, string numeroDomanda, byte? progStorico, string certificato, DateTime dataPresentazioneDomanda,
                DateTime? dataElaborazione, bool? indConvInt, string gestione, string matricola, string descProdotto, string descTipo)
            {
                _Nome = nome;
                _Cognome = cognome;
                _CodiceFiscale = codiceFiscale;
                _Sede = sede;
                _CentroOperativo = centroOperativo;
                _Categoria = categoria;
                _Tipo = tipo;
                _Fondo = fondo;
                _Stato = decStatoPensione;
                _NumeroDomanda = numeroDomanda;
                _ProgStorico = progStorico;
                _Certificato = certificato;
                _DataPresentazioneDomanda = dataPresentazioneDomanda;
                _DataElaborazioneDomanda = dataElaborazione;
                _TipoAppartenenza = Utility.GetTipoAppartenenza(indConvInt, gestione);
                _TipoFondo = Utility.GetTipoFondoByCategoria(indConvInt, gestione, categoria);
                _Matricola = matricola;
                _DescProdotto = descProdotto;
                _DescTipo = descTipo;
                _SedeDestinazione = sedeDestinazione;
                _CentroOperativoDestinazione = centroOperativoDestinazione;

            }

            #region public properties
            public string NumeroDomanda
            {
                get { return _NumeroDomanda; }
                set { _NumeroDomanda = value; }
            }

            public byte? ProgStorico { get { return _ProgStorico; } set { _ProgStorico = value; } }

            public string Categoria
            {
                get { return _Categoria; }
                set { _Categoria = value; }
            }

            public string Sede
            {
                get { return _Sede; }
                set { _Sede = value; }
            }

            public string CentroOperativo
            {
                get { return _CentroOperativo; }
                set { _CentroOperativo = value; }
            }

            public string Certificato
            {
                get { return _Certificato; }
                set { _Certificato = value; }
            }

            public string Tipo
            {
                get { return _Tipo; }
                set { _Tipo = value; }
            }

            public string Stato
            {
                get { return _Stato; }
                set { _Stato = value; }
            }

            public string Nome
            {
                get { return _Nome; }
                set { _Nome = value; }
            }

            public string Cognome
            {
                get { return _Cognome; }
                set { _Cognome = value; }
            }

            public string CodiceFiscale
            {
                get { return _CodiceFiscale; }
                set { _CodiceFiscale = value; }
            }

            public string Fondo
            {
                get { return _Fondo; }
                set { _Fondo = value; }
            }

            public DateTime DataPresentazioneDomanda
            {
                get { return _DataPresentazioneDomanda; }
                set { _DataPresentazioneDomanda = value; }
            }

            public DateTime? DataElaborazioneDomanda
            {
                get { return _DataElaborazioneDomanda; }
                set { _DataElaborazioneDomanda = value; }
            }
            public Utility.TipoAppartenenza? TipoAppartenenza
            {
                get { return _TipoAppartenenza; }
                set { _TipoAppartenenza = value; }
            }

            public Utility.TipoFondo? TipoFondo
            {
                get { return _TipoFondo; }
                set { _TipoFondo = value; }
            }

            public string Matricola
            {
                get { return _Matricola; }
                set { _Matricola = value; }
            }

            public string DescProdotto
            {
                get { return _DescProdotto; }
                set { _DescProdotto = value; }
            }

            public string DescTipo
            {
                get { return _DescTipo; }
                set { _DescTipo = value; }
            }

            public string SedeDestinazione
            {
                get { return _SedeDestinazione; }
                set { _SedeDestinazione = value; }
            }

            public string CentroOperativoDestinazione
            {
                get { return _CentroOperativoDestinazione; }
                set { _CentroOperativoDestinazione = value; }
            }
            #endregion public properties

            #region private properties
            private string _NumeroDomanda;
            private byte? _ProgStorico;
            private string _Categoria;
            private string _Sede;
            private string _CentroOperativo;
            private string _Certificato;
            private string _Tipo;
            private string _Stato;
            private string _Nome;
            private string _Cognome;
            private string _CodiceFiscale;
            private string _Fondo;
            private DateTime _DataPresentazioneDomanda;
            private DateTime? _DataElaborazioneDomanda;
            private Utility.TipoAppartenenza? _TipoAppartenenza;
            private Utility.TipoFondo? _TipoFondo;
            private string _Matricola;
            private string _DescProdotto;
            private string _DescTipo;
            private string _SedeDestinazione;
            private string _CentroOperativoDestinazione;
            #endregion private properties

        }
        #endregion nested class

        public enum TipoDomanda
        {
            Nessuno,
            PL,
            RIC,
            PL_RIC
        }
    }
}
