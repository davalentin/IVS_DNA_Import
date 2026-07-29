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
    /// Summary description for AnagraficaTest
    /// </summary>
    [TestClass]
    public class AnagraficaTest
    {
        public AnagraficaTest()
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

        //Recupero anagrafica per codice fiscale
        [TestMethod]
        public void TestGetAnagraficaPerCodiceFiscale()
        {
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            //test soggetto presente
            GestioneAnagrafica.GetAnagraficaByCodiceFiscale("LTTDRV45B02H501E", out datiAnagrafici);
            Assert.IsTrue(datiAnagrafici != null, "Errore nel recupero dell'anagrafica");

            //test soggetto non presente
            GestioneAnagrafica.GetAnagraficaByCodiceFiscale("CZZPQL83R02C495E", out datiAnagrafici);
            Assert.IsTrue(datiAnagrafici == null, "Anagrafica attesa nulla per soggetto non presente");
        }

        //Recupero anagrafica per numero domanda
        [TestMethod]
        public void TestGetAnagraficaPerNumeroDomanda()
        {
            long idPensione = 0;
            GestionePensione.GetIdPensioneByNumeroDomanda(2038517500007, null, out idPensione);

            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
            //test anagrafica presente
            GestioneAnagrafica.GetAnagraficaByIdPensione(idPensione, out datiAnagrafici);
            Assert.IsTrue(datiAnagrafici != null, "Errore nel recupero dell'anagrafica");

            //test anagrafica non presente con domanda presente
            GestioneAnagrafica.GetAnagraficaByIdPensione(idPensione, out datiAnagrafici);
            Assert.IsTrue(datiAnagrafici == null, "Anagrafica attesa nulla per domanda presente");

            //test anagrafica non presente con domanda assente
            GestioneAnagrafica.GetAnagraficaByIdPensione(0, out datiAnagrafici);
            Assert.IsTrue(datiAnagrafici == null, "Anagrafica attesa nulla per domanda non presente");
        }

        //Recupero area titolare per numero domanda
        [TestMethod]
        public void TestGetAreaTitolarePerNumeroDomanda()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);

            BLCommon.Entity.AreaTitolare areaTitolare = null;
            Entity.Anagrafica anagrafica = null;
            String errori = "";
            //test area titolare presente
            bool bTest = GestioneAreaTitolare.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare, out anagrafica, out errori);
            Assert.IsTrue(bTest && areaTitolare != null, "Errore nel recupero dell'area titolare");
            //test area titolare presente
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare);
            Assert.IsTrue(bTest && areaTitolare != null, "Errore nel recupero dell'area titolare");
            //test area titolare non presente con domanda assente
            GestioneAnagrafica.GetAreaTitolareByDatiPensione(null, out areaTitolare);
            Assert.IsTrue(bTest && areaTitolare == null, "Area titolare attesa nulla per domanda non presente");
        }

        [TestMethod]
        public void TestStoreTabs_Anagrafica_StatoCivile_ResEstere()
        {
            GestionePensione.DatiPensione datiPensione = null;
            GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;

            GestionePensione.GetPensioneByNumeroDomandaAndProg(2005507600002, null, out datiPensione);
            GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);
            
            string errori = "";

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.GetTipoAppartenenza(datiPensione.IndConvInt, datiPensione.Gestione));

            BLCommon.Entity.AreaTitolare areaTitolare     = null;
            Entity.Anagrafica anagrafica                  = null;

            GestioneAreaTitolare.DeleteStatoCivileByDatiPensione(datiPensione, out errori);
            Assert.IsTrue(errori == string.Empty, "DeleteStatoCivile");
            GestioneAreaTitolare.DeleteResidenzeEstereByDatiPensione(datiPensione, out errori);
            Assert.IsTrue(errori == string.Empty, "DeleteResidenzeEstere");


            GestioneAreaTitolare.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare, out anagrafica, out errori);

            Pensioni.Liquidazione.BLCommon.GestioneQuadri.DatiQuadroTitolare datiQuadroTitolare = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);
            Assert.IsTrue(datiQuadroTitolare.TabAnagrafica == 0 && datiQuadroTitolare.TabResidenzeEstero == 1 &&
                          datiQuadroTitolare.TabStatiCivili == 0, "Semaforo Dati Tabs_Anagrafica_StatoCivile_ResEstere non corretto_A");

            Assert.IsTrue(areaTitolare.Patronato == null && areaTitolare.Sindacato == null &&
                          areaTitolare.Anagrafica.EMail == null && areaTitolare.Anagrafica.Tel == null && areaTitolare.Anagrafica.Cell == null, "Delete KO");


            areaTitolare.Anagrafica.Cell  = "3380001112";
            areaTitolare.Anagrafica.Tel   = "06111222333";
            areaTitolare.Anagrafica.EMail = "pippo@tin.it";

            areaTitolare.Pensione.DecorrenzaOriginaria = DateTime.Parse("27/08/2011 12:15:50");

            areaTitolare.Patronato = new GestionePensione.DatiPatronato("001", "AQ02", null, "02");

            areaTitolare.Sindacato = new GestionePensione.DatiSindacato("2", string.Empty, DateTime.Parse("29/08/2011 12:15:50"), DateTime.Parse("28/08/2011 12:15:50"), false);

            errori = "";
            bool isTabAnagraficaSaved = false;
            bool isWarning = false;
            GestioneAreaTitolare.SalvaAnagrafica(datiPensione, datiAnagrafici, areaTitolare, true, out isTabAnagraficaSaved, false, DateTime.Now, out isWarning, out errori);
            areaTitolare = null;
            GestioneAreaTitolare.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare, out anagrafica, out errori);

            datiQuadroTitolare = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);
            Assert.IsTrue(datiQuadroTitolare.TabAnagrafica == 2 && datiQuadroTitolare.TabResidenzeEstero == 1 &&
                          datiQuadroTitolare.TabStatiCivili == 0, "Semaforo Dati Tabs_Anagrafica_StatoCivile_ResEstere non corretto_B");

            Assert.IsTrue(areaTitolare.Patronato.CodiceEnte.Trim() == "001" && areaTitolare.Patronato.CodiceUfficio == "AQ02" && areaTitolare.Patronato.NPratica == null &&
                          areaTitolare.Patronato.TipoUfficio == "02" && areaTitolare.Sindacato.CessazioneSindacato == DateTime.Parse("28/08/2011 12:15:50") &&
                          areaTitolare.Sindacato.CodiceSindacato.Trim() == "2" && areaTitolare.Sindacato.DecorrenzaSindacato == DateTime.Parse("29/08/2011 12:15:50") &&
                          areaTitolare.Anagrafica.EMail == "pippo@tin.it" && areaTitolare.Anagrafica.Tel == "06111222333" && areaTitolare.Anagrafica.Cell == "3380001112"
                          && areaTitolare.Pensione.DecorrenzaOriginaria == DateTime.Parse("27/08/2011 12:15:50"), "SalvaAnagrafica_KO");

            long idAnagrafica = 0;
            GestioneAnagrafica.GetIdAnagraficaByCodiceFiscale(areaTitolare.Anagrafica.CodiceFiscale, out idAnagrafica);
            GestioneAnagrafica.DatiStatoCivile statoCivile = new GestioneAnagrafica.DatiStatoCivile(DateTime.Parse("29/08/2011 12:15:50"), '2');

            areaTitolare.ElencoStatiCivili = new List<GestioneAnagrafica.DatiStatoCivile>() { statoCivile };

            GestioneAreaTitolare.SalvaStatoCivile(datiPensione, areaTitolare, true, dataSistema, false, out errori);

            areaTitolare = null;
            GestioneAreaTitolare.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare, out anagrafica, out errori);

            datiQuadroTitolare = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);
            Assert.IsTrue(datiQuadroTitolare.TabAnagrafica == 2 && datiQuadroTitolare.TabResidenzeEstero == 1 &&
                          datiQuadroTitolare.TabStatiCivili == 2, "Semaforo Dati Tabs_Anagrafica_StatoCivile_ResEstere non corretto_C");

            Assert.IsTrue(areaTitolare.ElencoStatiCivili.Count == 1 && areaTitolare.ElencoStatiCivili[0].Codice == '2'
                && areaTitolare.ElencoStatiCivili[0].Decorrenza == DateTime.Parse("29/08/2011 12:15:50"), "SalvaStatoCivile_KO");

            GestioneAnagrafica.DatiResidenzaEstero ResidenzaEstero = new GestioneAnagrafica.DatiResidenzaEstero(DateTime.Parse("31/08/2011 12:15:50"), "KKKK");
            areaTitolare.ElencoResidenzeEstere = new List<GestioneAnagrafica.DatiResidenzaEstero>() { ResidenzaEstero };

            GestioneAreaTitolare.SalvaResidenzeEstereByDatiPensione(datiPensione, areaTitolare, true, out errori);

            datiQuadroTitolare = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);
            Assert.IsTrue(datiQuadroTitolare.TabAnagrafica == 2 && datiQuadroTitolare.TabResidenzeEstero == 2 &&
                          datiQuadroTitolare.TabStatiCivili == 2, "Semaforo Dati Tabs_Anagrafica_StatoCivile_ResEstere non corretto_D");

            Assert.IsTrue(areaTitolare.ElencoResidenzeEstere.Count == 1 && areaTitolare.ElencoResidenzeEstere[0].CodCatastaleStatoEE == "KKKK"
                && areaTitolare.ElencoResidenzeEstere[0].Decorrenza == DateTime.Parse("31/08/2011 12:15:50"), "SalvaResidenzaEstero_KO");

            GestioneAreaTitolare.DeleteStatoCivileByDatiPensione(datiPensione, out errori);
            Assert.IsTrue(errori == string.Empty);
            GestioneAreaTitolare.DeleteResidenzeEstereByDatiPensione(datiPensione, out errori);
            Assert.IsTrue(errori == string.Empty);


            GestioneAreaTitolare.GetAreaTitolareByDatiPensione(datiPensione, out areaTitolare, out anagrafica, out errori);

            datiQuadroTitolare = null;
            Pensioni.Liquidazione.BLCommon.GestioneQuadri.GetQuadroTitolareByDatiPensione(datiPensione, out datiQuadroTitolare);
            Assert.IsTrue(datiQuadroTitolare.TabAnagrafica == 0 && datiQuadroTitolare.TabResidenzeEstero == 1 &&
                          datiQuadroTitolare.TabStatiCivili == 0, "Semaforo Dati Tabs_Anagrafica_StatoCivile_ResEstere non corretto_D");

            Assert.IsTrue(areaTitolare.Patronato == null && areaTitolare.Sindacato == null &&
                          areaTitolare.Anagrafica.EMail == null && areaTitolare.Anagrafica.Tel == null && areaTitolare.Anagrafica.Cell == null, "Delete KO1");

        }

        [TestMethod()]
        public void TestControlsDatiAnagraficaDopoAggiornaARCA()
        {
            long numeroDomanda = 2038635900004;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            string codResidenzaDB = "C351";
            string codResidenzaARCA = "H501";
            bool isResidenzaEsteroDB = false;
            bool isResidenzaEsteroARCA = false;
            DateTime? dataMorteTitolare = null;
            Dictionary<Utility.TabAggArca, byte?> semafori = null;

            GestioneIstruttoria.DatiIstruttoria datiIstruttoria = null;
            GestioneIstruttoria.GetIstruttoriaByIdPensione(datiPensione.Id, out datiIstruttoria);

            GestioneDetrazioniImposta.DatiDetrazioni datiDetrazioni = null;
            GestioneDetrazioniImposta.GetDetrazioniByIdPensione(datiPensione.Id, out datiDetrazioni);

            GestioneDatiGenericiAgoCi.PensioniDatiGenerici datiPensioniDatiGenerici = null;
            GestioneDatiGenericiAgoCi.GetDatiGenericiByIdPensione(datiPensione.Id, out datiPensioniDatiGenerici);

            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            DateTime dataSistema = GestioneControlliDinamici.GetDataSistema(Utility.TipoAppartenenza.AGO);

            GestioneAreaTitolare.ControlsDatiAnagraficaDopoAggiornaARCA(datiPensione, datiIstruttoria, datiDetrazioni, datiPensioniDatiGenerici, Utility.TipoAppartenenza.AGO, codResidenzaARCA, codResidenzaDB, isResidenzaEsteroARCA, 
                isResidenzaEsteroDB, dataMorteTitolare, isRiaperturaDomanda, dataSistema, out semafori);
        }
    }
}
