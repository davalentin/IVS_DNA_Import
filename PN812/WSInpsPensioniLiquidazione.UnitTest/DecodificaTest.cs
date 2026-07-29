using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for DecodificaTest
    /// </summary>
    [TestClass]
    public class DecodificaTest
    {
        public DecodificaTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion Additional test attributes

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            RoleApplications roleApplications;
            RoleApplication roleApplication;
            IdmIdentity ident;

            ident = new IdmIdentity("4444444444", "ABBCDA74A10H501B", "Mario", "Rossi", "mrossi", @"domain\mrossi", "mrossi@inps.it", "0600", "060000");

            roleApplications = new RoleApplications();
            roleApplication = roleApplications.Add("Dominio1.Gruppo1.Processo1");
            roleApplication.Add("Ruolo1");

            System.Threading.Thread.CurrentPrincipal = new IdmPrincipal(ident, roleApplications);

            INPS.DNA.Context.OperationContextInfo.CreateUnitTestContext();
            INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.Offices["060000"];
        }

        //Recupero patronato
        [TestMethod]
        public void TestGetPatronato()
        {
            BLCommon.GestioneDecodifica.Patronato patronato = null;
            GestioneDecodifica.GetPatronatoByEnteUfficio("001", "AQ02", out patronato);
            if (patronato == null)
                Assert.Fail("Errore nel recupero del patronato");

        }

        //Recupero stati civili
        [TestMethod]
        public void TestGetStatiCivili()
        {
            List<BLCommon.GestioneDecodifica.StatoCivile> elencoStatiCivili = null;
            GestioneDecodifica.GetStatiCivili(out elencoStatiCivili);
            if (elencoStatiCivili == null)
                Assert.Fail("Errore nel recupero degli stati civili");
        }

        //Recupero stati esteri
        [TestMethod]
        public void TestGetStatiEsteri()
        {
            List<BLCommon.GestioneDecodifica.StatoEstero> elencoStatiEsteri = null;
            GestioneDecodifica.GetStatiEsteri(out elencoStatiEsteri);
            if (elencoStatiEsteri == null)
                Assert.Fail("Errore nel recupero degli stati esteri");
        }

        //Recupero stato estero per codice catastale
        [TestMethod]
        public void TestGetStatoEsteroByCodCatastale()
        {
            BLCommon.GestioneDecodifica.StatoEstero statoEstero = null;
            GestioneDecodifica.GetStatoEsteroPerCodiceCatastale("Z130", out statoEstero);
            if (statoEstero == null)
                Assert.Fail("Errore nel recupero dello stato estero");
        }

        //Recupero province
        [TestMethod]
        public void TestGetProvince()
        {
            List<BLCommon.GestioneDecodifica.Provincia> elencoProvince = null;
            GestioneDecodifica.GetProvince(out elencoProvince);
            if (elencoProvince == null)
                Assert.Fail("Errore nel recupero delle province");
        }

        //Recupero comuni per provincia
        [TestMethod]
        public void TestGetComuniPerProvincia()
        {
            List<BLCommon.GestioneDecodifica.Comune> elencoComuni = null;
            GestioneDecodifica.GetComuniPerProvincia("NA", out elencoComuni);
            if (elencoComuni == null)
                Assert.Fail("Errore nel recupero dei comuni per provincia");

            GestioneDecodifica.GetComuniPerProvincia("AA", out elencoComuni);
            if (elencoComuni != null)
                Assert.Fail("Errore nel recupero dei comuni per provincia");
        }

        //Recupero coniugeOFiglio
        [TestMethod]
        public void TestGetConiugeOFiglio()
        {
            List<BLCommon.GestioneDecodifica.ConiugeOFiglio> elencoConiugeOFiglio = null;
            GestioneDecodifica.GetConiugeOFiglio(out elencoConiugeOFiglio);
            if (elencoConiugeOFiglio == null)
                Assert.Fail("Errore nel recupero dei coniugeOFiglio");
        }

        //Recupero detrazioniReddito
        [TestMethod]
        public void TestGetDetrazioniReddito()
        {
            List<BLCommon.GestioneDecodifica.DetrazioniReddito> elencoDetrazioniReddito = null;
            GestioneDecodifica.GetDetrazioniReddito(out elencoDetrazioniReddito);
            if (elencoDetrazioniReddito == null)
                Assert.Fail("Errore nel recupero dei detrazioniReddito");
        }

        //Recupero tutore
        [TestMethod]
        public void TestGetTutore()
        {
            List<BLCommon.GestioneDecodifica.Tutore> elencoTutore = null;
            GestioneDecodifica.GetTutore(out elencoTutore);
            if (elencoTutore == null)
                Assert.Fail("Errore nel recupero del tutore");
        }

        //Recupero delegato
        [TestMethod]
        public void TestGetDelegato()
        {
            List<BLCommon.GestioneDecodifica.Delegato> elencoDelegato = null;
            GestioneDecodifica.GetDelegato(out elencoDelegato);
            if (elencoDelegato == null)
                Assert.Fail("Errore nel recupero del delegato");
        }

        //Recupero sigla familiare
        [TestMethod]
        public void TestGetSiglaFamiliare()
        {
            List<BLCommon.GestioneDecodifica.SiglaFamiliare> elencoSiglaFamiliare = null;
            GestioneDecodifica.GetSiglaFamiliareByTipologia("FS", out elencoSiglaFamiliare);
            if (elencoSiglaFamiliare == null)
                Assert.Fail("Errore nel recupero della sigla familiare");
        }

        //Recupero modalitaPagamento
        [TestMethod]
        public void TestGetModalitaPagamento()
        {
            List<BLCommon.GestioneDecodifica.ModalitaPagamento> elencoModalitaPagamento = null;
            GestioneDecodifica.GetModalitaPagamento(out elencoModalitaPagamento);
            if (elencoModalitaPagamento == null)
                Assert.Fail("Errore nel recupero della modalitaPagamento");
        }

        //Recupero tipoPagamento
        [TestMethod]
        public void TestGetTipoPagamento()
        {
            List<BLCommon.GestioneDecodifica.TipoPagamento> elencoTipoPagamento = null;
            GestioneDecodifica.GetTipoPagamento(out elencoTipoPagamento);
            if (elencoTipoPagamento == null)
                Assert.Fail("Errore nel recupero del tipoPagamento");
        }

        //Recupero tipoCalcolo
        [TestMethod]
        public void TestGetTipoCalcolo()
        {
            List<BLCommon.GestioneDecodifica.TipoCalcolo> elencoTipoCalcolo = null;
            GestioneDecodifica.GetTipoCalcolo(out elencoTipoCalcolo);
            if (elencoTipoCalcolo == null)
                Assert.Fail("Errore nel recupero del tipoCalcolo");
        }

        //Recupero causaCarico
        [TestMethod]
        public void TestGetCausaCarico()
        {
            List<BLCommon.GestioneDecodifica.CausaCarico> elencoCausaCarico = null;
            GestioneDecodifica.GetCausaCarico(out elencoCausaCarico);
            if (elencoCausaCarico == null)
                Assert.Fail("Errore nel recupero del causaCarico");
        }

        //Recupero codiceEliminazione
        [TestMethod]
        public void TestGetCodiceEliminazione()
        {
            List<BLCommon.GestioneDecodifica.CodiceEliminazione> elencoCodiceEliminazione = null;
            GestioneDecodifica.GetCodiceEliminazioneByTipologia(out elencoCodiceEliminazione,Utility.TipoAppartenenza.AGO);
            if (elencoCodiceEliminazione == null)
                Assert.Fail("Errore nel recupero del codiceEliminazione");
        }

        //Recupero attivitaSvolta per fondo
        [TestMethod]
        public void TestGetAttivitaSvoltaByFondo()
        {
            List<BLCommon.GestioneDecodifica.AttivitaSvolta> elencoAttivitaSvolta = null;
            //dati presenti
            GestioneDecodifica.GetAttivitaSvoltaByFondo("EL", null, out elencoAttivitaSvolta);
            if (elencoAttivitaSvolta == null)
                Assert.Fail("Errore nel recupero del attivitaSvolta per fondo");

            //dati non presenti
            elencoAttivitaSvolta = null;
            GestioneDecodifica.GetAttivitaSvoltaByFondo("", null, out elencoAttivitaSvolta);
            if (elencoAttivitaSvolta != null)
                Assert.Fail("AttivitaSvolta per fondo non nulla o vuota");
        }

        // Recupero attivitaSvolta per id
        [TestMethod]
        public void TestGetAttivitaSvoltaById()
        {
            BLCommon.GestioneDecodifica.AttivitaSvolta attivitaSvolta = null;
            GestioneDecodifica.GetAttivitaSvoltaById("110", out attivitaSvolta);
            //dati presenti
            if (attivitaSvolta == null)
                Assert.Fail("Errore nel recupero del attivitaSvolta per id");

            //dati non presenti
            attivitaSvolta = null;
            GestioneDecodifica.GetAttivitaSvoltaById("999", out attivitaSvolta);
            if (attivitaSvolta != null)
                Assert.Fail("AttivitaSvolta per id non nulla o vuota");
        }

        //Recupero codiceCristallizzazione
        [TestMethod]
        public void TestGetCodiceCristallizzazione()
        {
            List<BLCommon.GestioneDecodifica.CodiceCristallizzazione> elencoCodiceCristallizzazione = null;
            GestioneDecodifica.GetCodiceCristallizzazione(out elencoCodiceCristallizzazione);
            if (elencoCodiceCristallizzazione == null)
                Assert.Fail("Errore nel recupero del codiceCristallizzazione");
        }

        //Recupero tipoPensione
        [TestMethod]
        public void TestGetTipoPensione()
        {
            List<BLCommon.GestioneDecodifica.TipoPensione> elencoTipoPensione = null;
            GestioneDecodifica.GetTipoPensione(out elencoTipoPensione);
            if (elencoTipoPensione == null)
                Assert.Fail("Errore nel recupero del tipoPensione");
        }

        //Recupero codiceAziendaEL
        [TestMethod]
        public void TestGetCodiceAziendaEL()
        {
            List<BLCommon.GestioneDecodifica.CodiceAzienda> elencoCodiceAziendaEL = null;
            GestioneDecodifica.GetCodiceAzienda(out elencoCodiceAziendaEL);
            if (elencoCodiceAziendaEL == null)
                Assert.Fail("Errore nel recupero del codiceAziendaEL");
        }

        //Recupero gradoInvalidita
        [TestMethod]
        public void TestGetGradoInvalidita()
        {
            List<BLCommon.GestioneDecodifica.GradoInvalidita> elencoGradoInvalidita = null;
            GestioneDecodifica.GetGradoInvalidita(out elencoGradoInvalidita);
            if (elencoGradoInvalidita == null)
                Assert.Fail("Errore nel recupero del gradoInvalidita");
        }

        //Recupero prorataEnel
        [TestMethod]
        public void TestGetProrataEnel()
        {
            List<BLCommon.GestioneDecodifica.ProrataEnel> elencoProrataEnel = null;
            GestioneDecodifica.GetProrataEnel(out elencoProrataEnel);
            if (elencoProrataEnel == null)
                Assert.Fail("Errore nel recupero del prorataEnel");
        }

        //Recupero gestione e fondo 
        [TestMethod]
        public void TestGetGestioneFondo()
        {
            string gestione = "";
            string fondo = "";
            //record presente
            GestioneDecodifica.GetGestioneFondoInChiaro("001", "001", out gestione, out fondo);
            if ((String.IsNullOrEmpty(gestione) && String.IsNullOrEmpty(fondo)))
                Assert.Fail("Errore nel recupero delle descrizioni di gestione e fondo");

            gestione = "";
            fondo = "";
            //record non presente
            GestioneDecodifica.GetGestioneFondoInChiaro("000", "000", out gestione, out fondo);
            if ((!String.IsNullOrEmpty(gestione) || !String.IsNullOrEmpty(fondo)))
                Assert.Fail("Descrizioni gestione e fondo non nulle");
        }

        //Recupero prodotto 
        [TestMethod]
        public void TestGetProdotto()
        {
            string prodotto = "";
            //record presente
            GestioneDecodifica.GetProdottoInChiaro("0001", out prodotto);
            if (String.IsNullOrEmpty(prodotto))
                Assert.Fail("Errore nel recupero del prodotto in chiaro");

            prodotto = "";
            //record non presente
            GestioneDecodifica.GetProdottoInChiaro("9999", out prodotto);
            if (!String.IsNullOrEmpty(prodotto))
                Assert.Fail("Prodotto in chiaro non nullo");
        }

        //Recupero tipologia 
        [TestMethod]
        public void TestGetTipologia()
        {
            string tipologia = "";
            //record presente
            GestioneDecodifica.GetTipologiaInChiaro("0001", out tipologia);
            if (String.IsNullOrEmpty(tipologia))
                Assert.Fail("Errore nel recupero del prodotto in chiaro");

            tipologia = "";
            //record non presente
            GestioneDecodifica.GetTipologiaInChiaro("9999", out tipologia);
            if (!String.IsNullOrEmpty(tipologia))
                Assert.Fail("Prodotto in chiaro non nullo");
        }

        //Recupero ente 
        [TestMethod]
        public void TestGetEnte()
        {
            string ente = "";
            //record presente
            GestioneDecodifica.GetEnteInChiaro("01", out ente);
            if (String.IsNullOrEmpty(ente))
                Assert.Fail("Errore nel recupero del prodotto in chiaro");

            ente = "";
            //record non presente
            GestioneDecodifica.GetEnteInChiaro("99", out ente);
            if (!String.IsNullOrEmpty(ente))
                Assert.Fail("Prodotto in chiaro non nullo");
        }

        //Recupero categoria numerica da sigla categoria 
        [TestMethod]

        public void TestGetCategoriaNumericaBySiglaCategoria()
        {
            string catNum = "";
            //record presente
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria("VO", out catNum);
            if (String.IsNullOrEmpty(catNum))
                Assert.Fail("Errore nel recupero della categoria numerica");

            GestioneDecodifica.GetCodCategoriaBySiglaCategoria("VEL", out catNum);
            if (String.IsNullOrEmpty(catNum))
                Assert.Fail("Errore nel recupero della categoria numerica");
            catNum = "";
            //record non presente
            GestioneDecodifica.GetCodCategoriaBySiglaCategoria("ZZ", out catNum);
            if (!String.IsNullOrEmpty(catNum))
                Assert.Fail("Categoria numerica non nulla");
        }

        //Recupero codiceComunicazioneCampi1_2
        [TestMethod]
        public void TestGetCodiceComunicazioneCampi1_2()
        {
            List<BLCommon.GestioneDecodifica.ComunicazioneCampi1_2> elencoCodiceComunicazioneCampi1_2 = null;
            GestioneDecodifica.GetComunicazioneCampi1_2(out elencoCodiceComunicazioneCampi1_2);
            if (elencoCodiceComunicazioneCampi1_2 == null)
                Assert.Fail("Errore nel recupero della codiceComunicazioneCampo3");
        }

        //Recupero codiceComunicazioneCampo3
        [TestMethod]
        public void TestGetComunicazioneCampo3()
        {
            List<BLCommon.GestioneDecodifica.ComunicazioneCampo3> elencoComunicazioneCampo3 = null;
            GestioneDecodifica.GetComunicazioneCampo3(out elencoComunicazioneCampo3);
            if (elencoComunicazioneCampo3 == null)
                Assert.Fail("Errore nel recupero della codiceComunicazioneCampo3");
        }

        //Recupero codiceComunicazioneCampo4
        [TestMethod]
        public void TestGetComunicazioneCampo4()
        {
            List<BLCommon.GestioneDecodifica.ComunicazioneCampo4> elencoComunicazioneCampo4 = null;
            GestioneDecodifica.GetComunicazioneCampo4(out elencoComunicazioneCampo4);
            if (elencoComunicazioneCampo4 == null)
                Assert.Fail("Errore nel recupero della codiceComunicazioneCampo4");
        }

        //Recupero codiciNatura
        [TestMethod]
        public void TestGetCodiciNatura()
        {
            List<BLCommon.GestioneDecodifica.CodiciNatura> elencoCodiciNatura = null;
            GestioneDecodifica.GetCodiciNatura(out elencoCodiciNatura);
            if (elencoCodiciNatura == null)
                Assert.Fail("Errore nel recupero della codiciNatura");
        }

        //Recupero codiceCieco
        [TestMethod]
        public void TestGetCodiceCieco()
        {
            List<BLCommon.GestioneDecodifica.Cieco> elencoCodiciCieco = null;
            GestioneDecodifica.GetCodiceCieco(out elencoCodiciCieco);
            if (elencoCodiciCieco == null)
                Assert.Fail("Errore nel recupero del codiceCieco");
        }

        //Recupero tipoSettimaneBeneficio
        [TestMethod]
        public void TestGetTipoSettimaneBeneficio()
        {
            List<BLCommon.GestioneDecodifica.SettimaneBeneficio> elencoTipoSettimaneBeneficio = null;
            GestioneDecodifica.GetTipoSettimaneBeneficio(out elencoTipoSettimaneBeneficio);
            if (elencoTipoSettimaneBeneficio == null)
                Assert.Fail("Errore nel recupero del tipoSettimaneBeneficio");
        }

        //Recupero tipoSupplementi
        [TestMethod]
        public void TestGetTipoSupplementi()
        {
            List<BLCommon.GestioneDecodifica.TipoSupplementi> elencoTipoSupplementi = null;
            GestioneDecodifica.GetTipoSupplementi(out elencoTipoSupplementi);
            if (elencoTipoSupplementi == null)
                Assert.Fail("Errore nel recupero del tipoSupplementi");
        }

        //Recupero codiciRequisitiParticolari
        [TestMethod]
        public void TestGetCodiciRequisitiParticolari()
        {
            List<BLCommon.GestioneDecodifica.CodiceRequisitoParticolare> elencoCodiciRequisitiParticolari = null;
            GestioneDecodifica.GetCodiciRequisitiParticolari(out elencoCodiciRequisitiParticolari);
            if (elencoCodiciRequisitiParticolari == null)
                Assert.Fail("Errore nel recupero dei codiciRequisitiParticolari");
        }

        //Recupero codiceMobilita
        [TestMethod]
        public void TestGetCodiceMobilita()
        {
            List<BLCommon.GestioneDecodifica.Mobilita> elencoCodiceMobilita = null;
            GestioneDecodifica.GetCodiceMobilita(out elencoCodiceMobilita);
            if (elencoCodiceMobilita == null)
                Assert.Fail("Errore nel recupero del codiceMobilita");
        }

        //Recupero codiceRequisito1
        [TestMethod]
        public void TestGetCodiceRequisito1()
        {
            List<BLCommon.GestioneDecodifica.CodiceRequisito1> elencoCodiceRequisito1 = null;
            GestioneDecodifica.GetCodiceRequisito1(out elencoCodiceRequisito1);
            if (elencoCodiceRequisito1 == null)
                Assert.Fail("Errore nel recupero del codiceRequisito1");
        }

        //Recupero codiceRequisito1
        [TestMethod]
        public void TestGetCodiceRequisito2()
        {
            List<BLCommon.GestioneDecodifica.CodiceRequisito2> elencoCodiceRequisito2 = null;
            GestioneDecodifica.GetCodiceRequisito2(out elencoCodiceRequisito2);
            if (elencoCodiceRequisito2 == null)
                Assert.Fail("Errore nel recupero del codiceRequisito1");
        }

        //Recupero codiceSpecifico
        [TestMethod]
        public void TestGetCodiceSpecifico()
        {
            List<BLCommon.GestioneDecodifica.CodiceSpecifico> elencoCodiceSpecifico = null;
            GestioneDecodifica.GetCodiceSpecifico(out elencoCodiceSpecifico);
            if (elencoCodiceSpecifico == null)
                Assert.Fail("Errore nel recupero del codiceSpecifico");
        }

        //Recupero codiceConvenzioneInternazionale
        [TestMethod]
        public void TestGetCodiceConvenzioneInternazionale()
        {
            List<BLCommon.GestioneDecodifica.CodiceConvenzioneInternazionale> elencoCodiceConvenzioneInternazionale = null;
            GestioneDecodifica.GetCodiceConvenzioneInternazionale(out elencoCodiceConvenzioneInternazionale);
            if (elencoCodiceConvenzioneInternazionale == null)
                Assert.Fail("Errore nel recupero del codiceConvenzioneInternazionale");
        }

        //Recupero codiceRequisitiLegge50392
        [TestMethod]
        public void TestGetCodiceRequisitiLegge50392()
        {
            List<BLCommon.GestioneDecodifica.CodiceRequisitiLegge50392> elencoCodiceRequisitiLegge50392 = null;
            GestioneDecodifica.GetCodiceRequisitiLegge50392(out elencoCodiceRequisitiLegge50392);
            if (elencoCodiceRequisitiLegge50392 == null)
                Assert.Fail("Errore nel recupero del codiceRequisitiLegge50392");
        }

        //Recupero codiceConvenzione
        [TestMethod]
        public void TestGetCodiceConvenzione()
        {
            List<BLCommon.GestioneDecodifica.CodiceConvenzione> elencoCodiceConvenzione = null;
            GestioneDecodifica.GetCodiceConvenzione(out elencoCodiceConvenzione);
            if (elencoCodiceConvenzione == null)
                Assert.Fail("Errore nel recupero del codiceConvenzione");
        }

        //Recupero codiceVirtuale
        [TestMethod]
        public void TestGetCodiceVirtuale()
        {
            List<BLCommon.GestioneDecodifica.CodiceVirtuale> elencoCodiceVirtuale = null;
            GestioneDecodifica.GetCodiceVirtuale(out elencoCodiceVirtuale);
            if (elencoCodiceVirtuale == null)
                Assert.Fail("Errore nel recupero del codiceVirtuale");
        }

        //Recupero regimeLiquidazione
        [TestMethod]
        public void TestGetRegimeLiquidazione()
        {
            List<BLCommon.GestioneDecodifica.RegimeLiquidazione> elencoRegimeLiquidazione = null;
            GestioneDecodifica.GetRegimeLiquidazione(out elencoRegimeLiquidazione);
            if (elencoRegimeLiquidazione == null)
                Assert.Fail("Errore nel recupero del regimeLiquidazione");
        }

        //Recupero TipoCalcoloSecondario
        [TestMethod]
        public void TestGetTipoCalcoloSecondario()
        {
            List<BLCommon.GestioneDecodifica.TipoCalcoloSecondario> elencoTipoCalcoloSecondario = null;
            GestioneDecodifica.GetTipoCalcoloSecondario(out elencoTipoCalcoloSecondario);
            if (elencoTipoCalcoloSecondario == null)
                Assert.Fail("Errore nel recupero del regimeLiquidazione");
        }

        //Recupero Codice Categoria dalla SiglaCategoria
        [TestMethod]
        public void TestGetCodCategoriaBySiglaCategoria()
        {
            List<GestioneDecodifica.CategoriaPensione> listaSiglaCategoria = null;
            GestioneDecodifica.GetCategoriePensione(out listaSiglaCategoria);

            foreach(GestioneDecodifica.CategoriaPensione categoria in listaSiglaCategoria)
            {
                string codCategoria = string.Empty;
                GestioneDecodifica.GetCodCategoriaBySiglaCategoria(categoria.SiglaCatPensione.PadRight(8, ' '), out codCategoria);
                Assert.IsFalse(categoria.CodCatPensione != codCategoria, "Categoria non trovata" + categoria.CodCatPensione);
            }
        }

        [TestMethod]
        public void TestGetSiglaFamiliareByParentela()
        {
            string relazioneParentela = "01";
            char? siglaFamiliare = null;
            string tipoUnione = null;
            GestioneDecodifica.GetSiglaFamiliareByParentela(relazioneParentela, out siglaFamiliare, out tipoUnione);
            Assert.IsFalse(siglaFamiliare == null, "Sigla familiare non trovata " + relazioneParentela);
        }
    }
}
