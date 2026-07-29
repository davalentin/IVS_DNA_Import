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
    /// Summary description for PensioneTest
    /// </summary>
    [TestClass]
    public class PensioneTest
    {
        public PensioneTest()
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

        //Recupero pensione per numero domanda
        [TestMethod]
        public void TestGetPensionePerNumeroDomanda()
        {
            GestionePensione.DatiPensione datiPensione = null;
            //test pensione presente
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2038517500007, null, out datiPensione);
            Assert.IsTrue(datiPensione != null, "Errore nel recupero della pensione");

            //test pensione non presente
            GestionePensione.GetPensioneByNumeroDomandaAndProg(0, null, out datiPensione);
            Assert.IsTrue(datiPensione == null, "Pensione attesa nulla per numero domanda non presente");
        }

        //Recupero pensione per codice fiscale
        [TestMethod]
        public void TestGetPensioniPerCodiceFiscale()
        {
            List<GestionePensione.DatiPensione> elencoPensioni = null;
            //test pensioni presenti per codice fiscale presente
            GestionePensione.GetPensioniByCodiceFiscale("LTTDRV45B02H501E", out elencoPensioni);
            Assert.IsTrue(elencoPensioni != null && elencoPensioni.Count > 0, "Errore nel recupero della pensione");

            //test pensioni presenti per codice fiscale presente
            GestionePensione.GetPensioniByCodiceFiscale("DSDNLN42E52F475W", out elencoPensioni);
            Assert.IsTrue(elencoPensioni != null && elencoPensioni.Count > 0, "Errore nel recupero delle pensioni");

            //test pensioni non presenti con codice fiscale presente
            GestionePensione.GetPensioniByCodiceFiscale("CZZPQL83R02C495X", out elencoPensioni);
            Assert.IsTrue(elencoPensioni == null, "Pensione attesa nulla per codice fiscale presente");

            //test pensioni non presenti con codice fiscale assente
            GestionePensione.GetPensioniByCodiceFiscale("", out elencoPensioni);
            Assert.IsTrue(elencoPensioni == null, "Pensione attesa nulla per codice fiscale non presente");
        }

        //Recupero patronato per numero domanda
        [TestMethod]
        public void TestGetPatronatoPerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestionePensione.EliminaPatronato(idPensione);

            GestionePensione.DatiPatronato datiPatronato = new GestionePensione.DatiPatronato("001", "AQ02", null, "02");
            GestionePensione.SalvaPatronato(idPensione, datiPatronato);

            GestionePensione.DatiPatronato patronato = null;
            //test un patronato presente per numero domanda presente
            GestionePensione.GetPatronatoByIdPensione(idPensione, out patronato);

            GestionePensione.EliminaPatronato(idPensione);
            patronato = null;
            //test un patronato assente per numero domanda presente
            GestionePensione.GetPatronatoByIdPensione(idPensione, out patronato);
            Assert.IsTrue(patronato == null, "Patronato non nullo o vuoto");

            datiPatronato = new GestionePensione.DatiPatronato("001", "AQ02", null, "02");
            GestionePensione.SalvaPatronato(idPensione, datiPatronato);
        }

        //Recupero sindacato per numero domanda
        [TestMethod]
        public void TestGetSindacatoPerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestionePensione.EliminaSindacati(idPensione);

            GestionePensione.DatiSindacato datiSindacato = new GestionePensione.DatiSindacato("1", string.Empty, null, null, false);
            GestionePensione.SalvaSindacato(idPensione, datiSindacato);

            GestionePensione.DatiSindacato sindacato = null;
            //test un sindacato presente per numero domanda presente
            GestionePensione.GetSindacatoByIdPensione(idPensione, out sindacato);
            Assert.IsTrue(sindacato != null, "Errore nel recupero del sindacato");

            GestionePensione.EliminaSindacati(idPensione);

            sindacato = null;
            //test un sindacato assente per numero domanda presente
            GestionePensione.GetSindacatoByIdPensione(idPensione, out sindacato);
            Assert.IsTrue(sindacato == null, "Sindacato non nullo o vuoto");

            datiSindacato = new GestionePensione.DatiSindacato("1", string.Empty, null, null, false);
            GestionePensione.SalvaSindacato(idPensione, datiSindacato);
        }

        //Recupero eliminazione per numero domanda
        [TestMethod]
        public void TestGetEliminazionePerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestionePensione.EliminaEliminazione(idPensione);

            GestionePensione.DatiEliminazione datiEliminazione = new GestionePensione.DatiEliminazione(
                1, new DateTime(2011, 01, 01), null, new DateTime(2011, 02, 01), null, new DateTime(2011, 03, 01), 2, "q", null, null, null);
            GestionePensione.SalvaEliminazione(idPensione, datiEliminazione);

            GestionePensione.DatiEliminazione eliminazione = null;
            //test un' eliminazione presente per numero domanda presente
            GestionePensione.GetEliminazioneByIdPensione(idPensione, out eliminazione);

            GestionePensione.EliminaEliminazione(idPensione);

            GestionePensione.GetIdPensioneByNumeroDomanda(2125517900002, null, out idPensione);

            GestionePensione.EliminaEliminazione(idPensione);

            eliminazione = null;
            //test un'eliminazione assente per numero domanda presente
            GestionePensione.GetEliminazioneByIdPensione(idPensione, out eliminazione);
            Assert.IsTrue(eliminazione == null, "Eliminazione non nulla o vuota");

            datiEliminazione = new GestionePensione.DatiEliminazione(
                1, null, new DateTime(2011, 01, 01), null, new DateTime(2011, 02, 01), null, 2, "q", new DateTime(2011, 03, 01), null, null);
            GestionePensione.SalvaEliminazione(idPensione, datiEliminazione);

            GestionePensione.EliminaEliminazione(idPensione);
        }

        [TestMethod]
        public void TestDeletePensione()
        {
            INPS.Pensioni.Liquidazione.DataCommon.DAGestionePensione.EliminaPensione(111);
        }

        [TestMethod]
        public void TestDecorrenzaSuperiore()
        {
            DateTime? dataValidita = null;
            Utility.TipoAppartenenza? tipoAppartenenza = Utility.TipoAppartenenza.FS;

            if (!Utility.ControllaDataDecorrenzaSuperiore(new DateTime(2012, 11, 01), tipoAppartenenza, out dataValidita).Value)
                Assert.Fail();
            if (!Utility.ControllaDataDecorrenzaSuperiore(new DateTime(2012, 12, 01), tipoAppartenenza, out dataValidita).Value)
                Assert.Fail();
            if (Utility.ControllaDataDecorrenzaSuperiore(new DateTime(2013, 01, 01), tipoAppartenenza, out dataValidita).Value)
                Assert.Fail();
        }

        [TestMethod]
        public void TestDomandaLavorabile()
        {
            string codSituazione = "0001";
            string codFase = "";
            bool? indConvInt = false;
            string gestione = "001"; // 001
            string siglaCategoria = "VOS";
            string gruppo = "0001";
            string prodotto = "0001";
            string tipo = "0001";
            string codiceTipoRichiesta = "01";
            Utility.TipoAppartenenza tipoAppartenenza = Utility.TipoAppartenenza.CI;
            short codiceSede = 300;

            string errore = "";
            bool isDomandaLavorabilePerEccezione = GestioneCtrlBypassTipologieNonAbilitate.IsDomandaLavorabilePerEccezione(tipoAppartenenza, codiceSede,
                                       gruppo, prodotto, tipo, siglaCategoria, codiceTipoRichiesta, false);

            GestioneAreaRiepilogo.IsDomandaLavorabile(codSituazione, codFase, indConvInt, gestione, siglaCategoria, gruppo, prodotto, tipo, codiceTipoRichiesta, isDomandaLavorabilePerEccezione, out errore);
        }

        [TestMethod]
        public void TestGP1()
        {
            string numDomanda = "2212843500001";
            string chiavePensione = "001419014504777";
            INPS.Pensioni.Liquidazione.ServiceReferences.DatiPensioni.DatiTGP1Response risposta = null;
            string msgVideo;
            GestioneDatiPensioni.GetDatiTGP1ByChiavePensione(numDomanda, chiavePensione, out risposta, out msgVideo);
        }
    }
}
