using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INPS.DNA.Security.Roles;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;
using INPS.Pensioni.Liquidazione.BLCommon;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for AventiDirittoTest
    /// </summary>
    [TestClass]
    public class AventiDirittoTest
    {
        public AventiDirittoTest()
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
        public void AggiornaAventiDirittoFromWebDom()
        {
            long numeroDomanda = 2008714000008;
            string errori = string.Empty;

            Entity.ParametriARCA parametriArca = new Entity.ParametriARCA();
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            if (datiPensione != null)
            {
                GestioneAnagrafica.DatiAnagrafici datiAnagrafici = null;
                GestioneAnagrafica.GetAnagraficaByIdPensione(datiPensione.Id, out datiAnagrafici);

                GestioneAreaAventiDiritto.AggiornaAventiDirittoFromWebDom(parametriArca, datiPensione, datiAnagrafici.CodiceFiscale, out errori);
            }
        }

        [TestMethod]
        public void TestFamiliari()
        {
            long numeroDomanda = 2017912800001;
            string errori = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            List<GestioneFamiliari.Familiare> listafamiliari = null;
            List<GestioneAnagrafica.DatiAnagrafici> listaAnagrafica = null;
            GestioneFamiliari.GetFamiliariByIdPensione(datiPensione.Id, out listafamiliari, out listaAnagrafica);

            listafamiliari = listafamiliari.OrderBy(x => x.Progressivo).ToList();
        }
    }
}
