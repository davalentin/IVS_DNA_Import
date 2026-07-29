using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INPS.DNA.Logging;
using System.Transactions;
using INPS.DNA.Data;
using System.Linq.Expressions;
using System.Data.Common;

namespace INPS.Pensioni.Liquidazione.DataCommon
{
    public class DAGestioneStatoPratica
    {

        public static void GetPensioniByNumeroDomanda(long numeroDomanda, ref List<RisultatoRicerca> listRisultatoRicerca)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    if (listRisultatoRicerca == null)
                    {
                        // se la lista dei risultati è null, allora nessun criterio di ricerca è stato ancora applicato: la ricerca va effettuata sul db
                        // tramite linq-to-sql
                        PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString"));
                        listRisultatoRicerca = (from pensione in db.Pensiones
                                                where pensione.NDomus == numeroDomanda
                                                select new RisultatoRicerca
                                                (
                                                    pensione.NDomus,
                                                    pensione.ProgStorico,
                                                    pensione.SiglaCategoria,
                                                    pensione.CodiceSede,
                                                    pensione.CentroOperativo.HasValue ? (short)pensione.CentroOperativo.Value : (short)0,
                                                    pensione.CodiceSedeDestinazione,
                                                    pensione.CentroOperativoDestinazione,
                                                    pensione.NCertificato,
                                                    pensione.Tipo,
                                                    pensione.DecodificaStatoPensione.DecodificaStato,
                                                    pensione.StatoPensione,
                                                    pensione.Titolares.First().Anagrafica.Nome,
                                                    pensione.Titolares.First().Anagrafica.Cognome,
                                                    pensione.Titolares.First().Anagrafica.CodiceFiscale,
                                                    pensione.Fondo,
                                                    pensione.DataPresentazioneDomanda,
                                                    pensione.DataElaborazione,
                                                    pensione.IndConvInt,
                                                    pensione.Gestione,
                                                    pensione.Gruppo,
                                                    pensione.MatricolaUtenteAcquisizione,
                                                    pensione.Prodotto,
                                                    pensione.Lavorazione.CodFase,
                                                    pensione.CodiceSedeGP1ALZ6,
                                                    pensione.CentroOperativoGP1ALZ6,
                                                    pensione.TipoAutomazione,
                                                    pensione.CodiceSedeLavorazione
                                                )).ToList<RisultatoRicerca>();
                        db.Connection.Close();
                        transactionScope.Complete();
                    }
                    // se la lista dei risultati è diversa da null e di molteplicità maggiore di 0, allora è già stato applicato un criterio
                    // di ricerca: il criterio corrente va applicato solo sulla lista dei risultati
                    // tramite linq-to-sql
                    else if (listRisultatoRicerca.Count > 0)
                    {
                        listRisultatoRicerca = (from lrr in listRisultatoRicerca
                                                where lrr.NumeroDomanda == numeroDomanda
                                                select lrr).ToList<RisultatoRicerca>();
                    }
                    // se la lista dei risultati ha molteplicità pari a 0, allora un criterio di ricerca è stato già applicato e non ha dato 
                    // alcun risultato: dal momento che i criteri di ricerca sono tutti in AND tra loro, non è necessario effettuare ulteriori 
                    // ricerche
                }
            }
        }

        public static void GetPensioniByExpression(Expression<Func<Pensione, bool>> predicatePensione, ref List<RisultatoRicerca> listRisultatoRicerca)
        {
            using (new MethodExecutionTracer())
            {
                using (TransactionScope transactionScope = TransactionScopeFactory.Create(TransactionScopeOption.Suppress))
                {
                    PensioniDataContext db = new PensioniDataContext(ConnectionFactory.GetConnection("PensioniConnectionString", ConnectionTypeEnum.Application));
                    var query = (from pensione in db.Pensiones
                                 select pensione);

                    listRisultatoRicerca = query.Where(predicatePensione).Select(pensione => new RisultatoRicerca
                                               (
                                                   pensione.NDomus,
                                                   pensione.ProgStorico,
                                                   pensione.SiglaCategoria,
                                                   pensione.CodiceSede,
                                                   pensione.CentroOperativo.HasValue ? (short)pensione.CentroOperativo.Value : (short)0,
                                                   pensione.CodiceSedeDestinazione,
                                                   pensione.CentroOperativoDestinazione,
                                                   pensione.NCertificato,
                                                   pensione.Tipo,
                                                   pensione.DecodificaStatoPensione.DecodificaStato,
                                                   pensione.StatoPensione,
                                                   pensione.Titolares.First().Anagrafica.Nome,
                                                   pensione.Titolares.First().Anagrafica.Cognome,
                                                   pensione.Titolares.First().Anagrafica.CodiceFiscale,
                                                   pensione.Fondo,
                                                   pensione.DataPresentazioneDomanda,
                                                   pensione.DataElaborazione,
                                                   pensione.IndConvInt,
                                                   pensione.Gestione,
                                                   pensione.Gruppo,
                                                   pensione.MatricolaUtenteAcquisizione,
                                                   pensione.Prodotto,
                                                   pensione.Lavorazione.CodFase,
                                                   pensione.CodiceSedeGP1ALZ6,
                                                   pensione.CentroOperativoGP1ALZ6,
                                                   pensione.TipoAutomazione,
                                                   pensione.CodiceSedeLavorazione
                                               )).ToList<RisultatoRicerca>();
                    db.Connection.Close();
                    transactionScope.Complete();
                }
            }
        }


        public class RisultatoRicerca
        {
            public RisultatoRicerca(long numeroDomanda, byte? progStorico, string categoria, short sede, short centroOperativo,
                short? sedeDestinazione, short? centroOperativoDestinazione, int? numeroCertificato, string tipo, string stato,
                short? codiceStato, string nome, string cognome, string codiceFiscale, string fondo, DateTime dataPresentazioneDomanda,
                DateTime? dataElaborazione, bool? indConvInt, string gestione, string gruppo, string matricola, string prodotto, string codFase, short? codiceSedeGP1ALZ6, byte? centroOperativoGP1ALZ6, byte? tipoAutomazione, short? codiceSedeLavorazione)
            {
                NumeroDomanda = numeroDomanda;
                ProgStorico = progStorico;
                Categoria = categoria;
                Sede = sede;
                CentroOperativo = centroOperativo;
                Certificato = numeroCertificato;
                Tipo = tipo;
                Stato = stato;
                CodiceStato = codiceStato;
                Nome = nome;
                Cognome = cognome;
                CodiceFiscale = codiceFiscale;
                Fondo = fondo;
                DataPresentazioneDomanda = dataPresentazioneDomanda;
                DataElaborazioneDomanda = dataElaborazione;
                IndConvInt = indConvInt;
                Gestione = gestione;
                Gruppo = gruppo;
                MatricolaUtenteAcquisizione = matricola;
                Prodotto = prodotto;
                CodFase = codFase;
                SedeDestinazione = sedeDestinazione;
                CentroOperativoDestinazione = centroOperativoDestinazione;
                CodiceSedeGP1ALZ6 = codiceSedeGP1ALZ6;
                CentroOperativoGP1ALZ6 = centroOperativoGP1ALZ6;
                TipoAutomazione = tipoAutomazione;
                CodiceSedeLavorazione = codiceSedeLavorazione;
            }

            #region public properties
            public long NumeroDomanda { get; set; }
            public byte? ProgStorico { get; set; }
            public string Categoria { get; set; }
            public short Sede { get; set; }
            public short CentroOperativo { get; set; }
            public int? Certificato { get; set; }
            public string Tipo { get; set; }
            public string Stato { get; set; }
            public short? CodiceStato { get; set; }
            public string Nome { get; set; }
            public string Cognome { get; set; }
            public string CodiceFiscale { get; set; }
            public string Fondo { get; set; }
            public DateTime DataPresentazioneDomanda { get; set; }
            public DateTime? DataElaborazioneDomanda { get; set; }
            public bool? IndConvInt { get; set; }
            public string Gestione { get; set; }
            public string Gruppo { get; set; }
            public string MatricolaUtenteAcquisizione { get; set; }
            public string Prodotto { get; set; }
            public string CodFase { get; set; }
            public short? SedeDestinazione { get; set; }
            public short? CentroOperativoDestinazione { get; set; }
            public short? CodiceSedeGP1ALZ6 { get; set; }
            public byte? CentroOperativoGP1ALZ6 { get; set; }
            public byte? TipoAutomazione { get; set; }
            public short? CodiceSedeLavorazione { get; set; }
            #endregion public properties
        }
    }
}
