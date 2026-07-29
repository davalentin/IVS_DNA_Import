using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Reflection;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for WebDomTest
    /// </summary>
    [TestClass]
    public class WebDomTest
    {
        public WebDomTest()
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

        [TestMethod]
        public void TestStatoDomandaWebDomInizioeChiusuraAttività()
        {
            long nDomusApertura = 2038562300008;
            long nDomusChiusura = 2038562300008;
            string matricola = "123456";
            string errori = string.Empty;
            short sede = 2100;

            GestionePensione.DatiPensione domApertura;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(nDomusApertura, null, out domApertura);

            GestionePensione.DatiPensione domChiusura;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(nDomusChiusura, null, out domChiusura);
            GestioneWebDom.ChiusuraAttivita(domChiusura, matricola, sede, GestioneWebDom.CodiceAttivita.InAcquisizione, out errori);
            GestioneWebDom.AperturaAttivita(domApertura, matricola, sede, GestioneWebDom.CodiceAttivita.AttesaCalcolo, out errori);

        }

        //Recupero domanda da WebDom per numero domanda
        [TestMethod]
        public void TestGetWebDomPerDomanda()
        {
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            string errori = "";
            //caso soggetto italiano
            bool bTest = GestioneWebDom.GetDomandaPerDomus("2038880100001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038517500007", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //caso soggetto estero
            bTest = GestioneWebDom.GetDomandaPerDomus("2125517900002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
            //caso domanda chiusa
            bTest = GestioneWebDom.GetDomandaPerDomus("2005507600002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //rev
            bTest = GestioneWebDom.GetDomandaPerDomus("2132523700001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2005401900001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038530500001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038503200002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038532800003", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038525800010", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //domanda con dati patronato
            bTest = GestioneWebDom.GetDomandaPerDomus("2038539200011", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ricRev1
            bTest = GestioneWebDom.GetDomandaPerDomus("2038518200007", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ricRev con dante causa senza pensione diretta 2038540700004
            bTest = GestioneWebDom.GetDomandaPerDomus("2038540700004", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ricRev con dante causa con pensione diretta 2038540700005
            bTest = GestioneWebDom.GetDomandaPerDomus("2038540700005", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //rev con dante causa senza pensione diretta 2038541000007
            bTest = GestioneWebDom.GetDomandaPerDomus("2038541000007", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
            //rev con dante causa con pensione diretta 2038523700001
            bTest = GestioneWebDom.GetDomandaPerDomus("2038523700001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //PL con nascita e residenza estera 2038540700006
            bTest = GestioneWebDom.GetDomandaPerDomus("2038540700006", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //VO
            bTest = GestioneWebDom.GetDomandaPerDomus("2009433500001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ric estera 2038542400003
            bTest = GestioneWebDom.GetDomandaPerDomus("2038542400003", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //caso numero domanda non valorizzato
            bTest = GestioneWebDom.GetDomandaPerDomus("", out datiDomanda, out errori);
            Assert.IsTrue(!bTest && errori != null && errori.Trim() != String.Empty, errori);
        }

        //Recupero domanda da WebDom per codice fiscale
        [TestMethod]
        public void TestGetWebDomPerCodiceFiscale()
        {
            List<ServiceReferences.WebDom.DatiDomanda> elencoDatiDomanda = null;
            string errori = "";
            bool bTest = GestioneWebDom.GetDomandePerCodiceFiscale("LTTDRV45B02H501E", "TI", out elencoDatiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandePerCodiceFiscale("", "TI", out elencoDatiDomanda, out errori);
            Assert.IsTrue(!bTest && errori != null && errori.Trim() != String.Empty, errori);
        }

        //Recupero domanda da WebDom per numero domanda, caso + soggetti da webdom
        [TestMethod]
        public void TestGetWebDomPerNumeroDomandaPiuSoggetti()
        {
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            string errori = "";
            //caso soggetto italiano
            bool bTest = GestioneWebDom.GetDomandaPerDomus("2005507600002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
        }

        //Salva dati anagrafici provenienti da WebDom
        [TestMethod]
        public void TestSalvaAnagraficaDaWebDom()
        {
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            string errori = "";
            bool bTest = GestioneWebDom.GetDomandaPerDomus("2038517500007", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.InsertAnagraficaFromWebDom(datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
        }


        [TestMethod]
        public void TestGetWebdomPerDomandeSpecifiche()
        {
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            string errori = "";
            bool bTest = GestioneWebDom.GetDomandaPerDomus("2212687500001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2005533400003", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038536800044", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038426200002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
            bTest = GestioneWebDom.GetDomandaPerDomus("2038523700002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038518200007", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038534800006", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038536800046", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038436900004", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2038550300003", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //rev. dir. Ci.
            bTest = GestioneWebDom.GetDomandaPerDomus("2005551200001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //rev. indir. Ci.
            bTest = GestioneWebDom.GetDomandaPerDomus("2005492200001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ric. rev. dir. ci.
            bTest = GestioneWebDom.GetDomandaPerDomus("2005551200002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
            //ric. rev. indir. ci. 
            bTest = GestioneWebDom.GetDomandaPerDomus("2005553200002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
            //ci generica VOS
            bTest = GestioneWebDom.GetDomandaPerDomus("2005491500001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago generica
            bTest = GestioneWebDom.GetDomandaPerDomus("2009447900002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago generica
            bTest = GestioneWebDom.GetDomandaPerDomus("2009555400002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago  ricRev diretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2146552500001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago  ricRev diretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2039552500001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago  ricRev indiretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2040552500001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago  ricRev indiretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2038552500005", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago rev diretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2008552500029", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago rev indiretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2009466800003", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //domanda patronato
            bTest = GestioneWebDom.GetDomandaPerDomus("2038536800028", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            bTest = GestioneWebDom.GetDomandaPerDomus("2008507000009", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago ric
            bTest = GestioneWebDom.GetDomandaPerDomus("2009555300003", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //ago rev
            bTest = GestioneWebDom.GetDomandaPerDomus("2009409600010", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //fs ric rev indir.
            bTest = GestioneWebDom.GetDomandaPerDomus("2038540700004", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //fs ric rev diretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2038540700005", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //fs rev diretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2038523700001", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //fs rev indiretta
            bTest = GestioneWebDom.GetDomandaPerDomus("2038554500003", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //fs normale TT
            bTest = GestioneWebDom.GetDomandaPerDomus("2038554000009", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

            //fs normale EL
            bTest = GestioneWebDom.GetDomandaPerDomus("2038458400005", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);

        }


        //public string GetNamePropertyByValue(object source,string valueProperty)
        //{
        //    string nameProperty = string.Empty;
        //    Type sourceType = source.GetType();
        //    PropertyInfo[] sourceProperties = sourceType.GetProperties();
        //    int i = 0;
        //    bool esci = false;
        //    List<PropertyInfo> nodeToExplore = new List<PropertyInfo>();
        //    while(esci)
        //    {
        //        PropertyInfo sourceProperty = sourceProperties[i];
        //        if (sourceProperty.ReflectedType == typeof(string))
        //        {
        //            if (sourceProperty.GetValue(source, null) == valueProperty)
        //            {
        //                return sourceProperty.Name;
        //            }
        //        }





        //    }
        //    return nameProperty;
        //}

        [TestMethod]
        public void TestGetWebdomPerAnalisiAttivita()
        {
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            string errori = "";
            bool bTest = GestioneWebDom.GetDomandaPerDomus("2008681800002", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
        }

        [TestMethod]
        public void TestAggiornaFaseAttivita()
        {
            string errori = string.Empty;
            INPS.Pensioni.Liquidazione.BLCommon.GestionePensione.DatiPensione datiPensione = null;
            INPS.Pensioni.Liquidazione.BLCommon.GestionePensione.GetPensioneByNumeroDomandaAndProg(2038560100001, null, out datiPensione);
            GestioneWebDom.AggiornamentoFaseAttivita(datiPensione, "12345678", 2100, out errori);
            Assert.IsTrue(string.IsNullOrEmpty(errori));
        }

        //Recupero domanda da WebDom per numero domanda
        [TestMethod]
        public void TestGetWebDomPerSingolaDomanda()
        {
            ServiceReferences.WebDom.DatiDomanda datiDomanda = null;
            string errori = "";
            //caso soggetto italiano
            bool bTest = GestioneWebDom.GetDomandaPerDomus("2008681700005", out datiDomanda, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
        }

        //SbloccaDomanda da WebDom per numero domanda
        [TestMethod]
        public void TestSbloccaWebDomPerSingolaDomanda()
        {
            string errori = "";
            bool bTest = GestioneWebDom.SbloccaDomandaWebDom(2038590100005, out errori);
            if (!bTest || !String.IsNullOrEmpty(errori))
                Assert.Fail(errori);
        }

        [TestMethod]
        public void TestAggiornaGestioneFondoEnte()
        {
            string errori = string.Empty;
            long numeroDomanda = 2008661400007;
            string codiceFondo = "002";
            string matricolaOperatore = "12345678";
            short sedeOperatore = 500;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            if (datiPensione != null)
            {
                bool bTest = GestioneWebDom.AggiornaFondoWebDom(datiPensione, matricolaOperatore, sedeOperatore, codiceFondo, out errori);
                if (!bTest || !string.IsNullOrEmpty(errori))
                    Assert.Fail(errori);
            }
        }


        [TestMethod]
        public void TestUtilityAggiornamentoWebDom()
        {
            GestioneAreaAggiornamento.ElaboraDomandeWebDom(Utility.TipoAppartenenza.AGO);

        }

        [TestMethod()]
        public void TestGetSedeDestinazione()
        {
            string codCatastale = "H501";
            string cap = "00176";
            string sedeDestinazione = null;
            string errori = string.Empty;
            GestioneWebDom.GetSedeDestinazione(0, codCatastale, cap, out sedeDestinazione, out errori);
        }

        [TestMethod()]
        public void TestGetCodUnitaProcesso()
        {
            short? codiceSedeDestinazione = 7000;
            byte? centroOperativoDestinazione = 0;
            byte? codUnitaProcesso = null;
            string gestione = "019";
            string errori = string.Empty;
            GestioneWebDom.GetCodUnitaProcesso(codiceSedeDestinazione, centroOperativoDestinazione, gestione, out codUnitaProcesso, out errori);
        }
    }
}
