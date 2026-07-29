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
    /// Summary description for CtrlBypassTipologieNonAbilitateTest
    /// </summary>
    [TestClass]
    public class CtrlBypassTipologieNonAbilitateTest
    {
        public CtrlBypassTipologieNonAbilitateTest()
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
        public void TestStoreCtrlBypassTipologieNonAbilitate()
        {
            string messaggioVideo = string.Empty;
            GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate areaCtrlBypassTipologieNonAbilitate = new GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate();
            areaCtrlBypassTipologieNonAbilitate.Tipologia = "FS";
            areaCtrlBypassTipologieNonAbilitate.Fondo = "PI";
            areaCtrlBypassTipologieNonAbilitate.Gruppo = "0001";
            areaCtrlBypassTipologieNonAbilitate.Prodotto = "0001";
            areaCtrlBypassTipologieNonAbilitate.Tipo = "0001";
            areaCtrlBypassTipologieNonAbilitate.Filtro = "222";
            areaCtrlBypassTipologieNonAbilitate.Categoria = "VPIA";
            areaCtrlBypassTipologieNonAbilitate.Sede = 2100;

            GestioneAreaCtrlBypassTipologieNonAbilitate.StoreCtrlBypassTipologieNonAbilitate(areaCtrlBypassTipologieNonAbilitate, out messaggioVideo);
        }

        [TestMethod]
        public void TestEliminaCtrlBypassTipologieNonAbilitate()
        {
            GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate areaCtrlBypassTipologieNonAbilitate = new GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate();
            areaCtrlBypassTipologieNonAbilitate.Tipologia = "FS";
            areaCtrlBypassTipologieNonAbilitate.Fondo = "PI";
            areaCtrlBypassTipologieNonAbilitate.Gruppo = "0001";
            areaCtrlBypassTipologieNonAbilitate.Prodotto = "0001";
            areaCtrlBypassTipologieNonAbilitate.Tipo = "0001";
            areaCtrlBypassTipologieNonAbilitate.Filtro = "B  ";
            areaCtrlBypassTipologieNonAbilitate.Categoria = "VPIA";
            areaCtrlBypassTipologieNonAbilitate.Sede = 2100;

            GestioneCtrlBypassTipologieNonAbilitate.EliminaCtrlBypassTipologieNonAbilitate(areaCtrlBypassTipologieNonAbilitate);
        }

        [TestMethod]
        public void TestSvuotaTabellaCtrlBypassTipologieNonAbilitate()
        {
            List<GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate> elencoCtrlBypassTipologieNonAbilitate;

            GestioneCtrlBypassTipologieNonAbilitate.GetCtrlBypassTipologieNonAbilitate(out elencoCtrlBypassTipologieNonAbilitate);

            foreach (GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate ctrl in elencoCtrlBypassTipologieNonAbilitate)
            {
                GestioneCtrlBypassTipologieNonAbilitate.EliminaCtrlBypassTipologieNonAbilitate(ctrl);
            }
        }

        [TestMethod]
        public void TestGetAllCtrlBypassTipologieNonAbilitate()
        {
            List<GestioneCtrlBypassTipologieNonAbilitate.CtrlBypassTipologieNonAbilitate> elencoCtrlBypassTipologieNonAbilitate;

            GestioneCtrlBypassTipologieNonAbilitate.GetCtrlBypassTipologieNonAbilitate(out elencoCtrlBypassTipologieNonAbilitate);
        }
    }
}
