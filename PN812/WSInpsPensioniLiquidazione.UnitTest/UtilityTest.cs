using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INPS.DNA.Security.Roles;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;
using INPS.Pensioni.Liquidazione.BLCommon;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for UtilityTest
    /// </summary>
    [TestClass]
    public class UtilityTest
    {
        public UtilityTest()
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
        public void TestDifferenzaBetweenDate()
        {
            DateTime? data1 = new DateTime(2012, 05, 30);
            DateTime? data2 = new DateTime(1956, 02, 29);

            INPS.Pensioni.Liquidazione.BLCommon.Utility.DifferenzaDateTime diff = Utility.DifferenzaBetweenDate(data1, data2, Utility.TipoAppartenenza.FS);
        }

        [TestMethod]
        public void TestDoubleComparison()
        {
            double a = 0.100;
            double b = 0.1;
            Assert.IsTrue(Utility.IsDoubleEquals(a,b));
        }

        [TestMethod()]
        public void TestAreEqualLists()
        {
            List<GestioneCalcolo.DatiCalcoloContributivo> list1 = null;
            List<GestioneCalcolo.DatiCalcoloContributivo> list2 = null;
            GestioneCalcolo.GetCalcoloContributivoCI_AGOByIdPensione(370245, out list1);
            GestioneCalcolo.GetCalcoloContributivoStoricoCI_AGOByIdPensione(370245, out list2);
            bool areEquals = Utility.AreEqualLists<GestioneCalcolo.DatiCalcoloContributivo>(list1, list2);
            List<GestioneCalcolo.DatiCalcoloRetributivo> list3 = null;
            List<GestioneCalcolo.DatiCalcoloRetributivo> list4 = null;
            GestioneCalcolo.GetCalcoloRetributivoCI_AGOByIdPensione(370245, out list3);
            GestioneCalcolo.GetCalcoloRetributivoStoricoCI_AGOByIdPensione(370245, out list4);
            bool areEquals2 = Utility.AreEqualLists<GestioneCalcolo.DatiCalcoloRetributivo>(list3, list4);
        }

        [TestMethod()]
        public void TestGetAAMMGGFromSettimane()
        {
            short settimane = 1085;
            short anni = 0;
            short mesi = 0;
            short giorni = 0;
            Utility.GetAAMMGGFromSettimane(settimane, out anni, out mesi, out giorni);

            Assert.IsTrue(settimane == (short)Math.Ceiling((anni * 52) +
                                                (mesi * 4.333) +
                                                (giorni / 6.923)));
        }

        internal static string basePath = @"C:\Liquidazione_pensioni\PN812\S1_WSINPSPENSIONILIQUIDAZIONE\WSInpsPensioniLiquidazione.UnitTest\XML\";

        #region internal private members
        internal static object Deserialize_Input(string fileName, object type)
        {
            XmlSerializer serializer = new XmlSerializer(type.GetType());

            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            object obj = serializer.Deserialize(reader);
            fs.Close();
            return obj;
        }
        #endregion internal private members
    }
}
