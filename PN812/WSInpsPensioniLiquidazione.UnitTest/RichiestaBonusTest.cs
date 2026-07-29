using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;

using INPS.Pensioni.Liquidazione;
using INPS.Pensioni.Liquidazione.BLCommon;
namespace INPS.Pensioni.Liquidazione.UnitTest
{
    [TestClass]
    public class RichiestaBonusTest
    {
        public RichiestaBonusTest() 
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
        public void TestPrenotazioneElaborazioni()
        {
            GestioneRichiestaBonus.AreaRichiestaBonus richiestaBonus = new GestioneRichiestaBonus.AreaRichiestaBonus();
            richiestaBonus.Certificato = "11677838";
            richiestaBonus.Categoria = "001";
            richiestaBonus.Sede = "4979";
            richiestaBonus.Anni = "2019|2020";
            richiestaBonus.TipoBonus = "BONUS14";
            richiestaBonus.NumDomanda = "2214878800001";
            string matricolaOperatore = "12345678";
            string sedeOperatore = "4979";
            long idPensione = 412385;
            GestioneRichiestaBonus.GetPrenotazioneElaborazioni(ref richiestaBonus, matricolaOperatore, sedeOperatore, idPensione);
            GestioneAnniRichiestaBonus.SalvaPrenotazioneElaborazioni(idPensione, richiestaBonus.DatiPrenotazioneElaborazioni);
            //Assert.IsTrue(datiEsitoCalcolo == null, "DatiEsitoCalcolo non nulli");
        }

        [TestMethod]
        public void TestQuadroRichiestaBonus()
        {
            GestioneRichiestaBonus.AreaRichiestaBonus richiestaBonus = new GestioneRichiestaBonus.AreaRichiestaBonus();
            richiestaBonus.Certificato = "11677838"; //"14361181";
            richiestaBonus.Categoria = "001";
            richiestaBonus.Sede = "4979"; // "7013";
            richiestaBonus.TipoBonus = "BONUS14";
            string numDomanda = "2214878800001";// "2156878800002";
            long idPensione = 0;
            GestioneRichiestaBonus.GetAnniDirittoAlBonus(ref richiestaBonus, numDomanda, idPensione);
        }
    }
}
