using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INPS.DNA.Security.Roles;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;
using INPS.Pensioni.Liquidazione.ServiceReferences.DatiPensioni;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for DatiPensioniTest
    /// </summary>
    [TestClass]
    public class DatiPensioniTest
    {
        public DatiPensioniTest()
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
        public void TestgetDatiTGP1()
        {
            DatiPensioniClient proxy = new DatiPensioniClient();
            DatiTGP1Request datiTGP1Request = new DatiTGP1Request();
            datiTGP1Request.ChiavePensione = "021210036033822";

            DatiTGP1Response response = proxy.GetDatiTGP1(datiTGP1Request);
        }

        [TestMethod]
        public void TestgetDatiTGP2()
        {
            DatiPensioniClient proxy = new DatiPensioniClient();
            DatiTGP2Request datiTGP2Request = new DatiTGP2Request();
            datiTGP2Request.ChiavePensione = "021210036033822";

            DatiTGP2Response response = proxy.GetDatiTGP2(datiTGP2Request);
        }

        [TestMethod]
        public void TestgetDatiTGP3()
        {
            DatiPensioniClient proxy = new DatiPensioniClient();
            DatiTGP3Request datiTGP3Request = new DatiTGP3Request();
            datiTGP3Request.ChiavePensione = "021210036033822";

            DatiTGP3Response response = proxy.GetDatiTGP3(datiTGP3Request);
        }

        [TestMethod]
        public void TestgetDatiTGP4()
        {
            DatiPensioniClient proxy = new DatiPensioniClient();
            DatiTGP4Request datiTGP4Request = new DatiTGP4Request();
            datiTGP4Request.ChiavePensione = "201700000342517";

            DatiTGP4Response response = proxy.GetDatiTGP4(datiTGP4Request);
        }

        [TestMethod]
        public void TestgetDatiTGP5()
        {
            DatiPensioniClient proxy = new DatiPensioniClient();
            DatiTGP5Request datiTGP5Request = new DatiTGP5Request();
            datiTGP5Request.ChiavePensione = "021210036033822";

            DatiTGP5Response response = proxy.GetDatiTGP5(datiTGP5Request);
        }

        [TestMethod]
        public void TestgetDatiTGP6()
        {
            DatiPensioniClient proxy = new DatiPensioniClient();
            DatiTGP6Request datiTGP6Request = new DatiTGP6Request();
            datiTGP6Request.ChiavePensione = "021210036033822";

            DatiTGP6Response response = proxy.GetDatiTGP6(datiTGP6Request);
        }

        [TestMethod]
        public void TestgetDatiTGP7()
        {
            DatiPensioniClient proxy = new DatiPensioniClient();
            DatiTGP7Request datiTGP7Request = new DatiTGP7Request();
            datiTGP7Request.ChiavePensione = "203999000000104";

            DatiTGP7Response response = proxy.GetDatiTGP7(datiTGP7Request);
        }

        [TestMethod]
        public void TestIsDomandaConPensioneLiquidata()
        {
            string errori = string.Empty;
            long nDomus = 2010634400001;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(nDomus, null, out datiPensione);
            bool isRiaperturaDomanda = Utility.IsRiaperturaDomanda(datiPensione.Id);

            bool bTest = GestioneDatiPensioni.IsDomandaConPensioneLiquidata(datiPensione, isRiaperturaDomanda, out errori);
            Assert.IsTrue(string.IsNullOrEmpty(errori));
            Assert.IsTrue(bTest);
        }
    }
}
