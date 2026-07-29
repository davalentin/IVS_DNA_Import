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
using System.Xml.Serialization;
using System.IO;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
    [TestClass]
    public class ANFTest
    {
        public ANFTest()
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



        [TestMethod()]
        public void RicercaDomandeANF()
        {
            string numdomanda = "1111111111111111";
            string codiceFiscale = "PRVLCU66H48D009W";
            string matricola = "123456789";
            string risposta = string.Empty;
            string errori = string.Empty;
            GestioneANF.RicercaDomandeANFByCodiceFiscale(numdomanda, codiceFiscale, matricola, out risposta, out errori);
        }

        [TestMethod()]
        public void RichiediRispostaAnf()
        {
            string numdomanda = "2008842800003";
            string codiceFiscale = "FRLNTN11B18M208B";
            string matricola = "123456789";
            string id = "F7E5E483-0BE8-4E3C-BC27-BCB4229BA6E5";
            string risposta = string.Empty;
            string errori = string.Empty;
            GestioneFamiliari.ConsultazioneUnificataANF consultazione = null;
            GestioneANF.RichiediRispostaById(numdomanda, codiceFiscale, id, matricola, out risposta, out errori);

            GestioneFamiliari.ControllaRispostaANF(risposta, out consultazione, out errori);
        }

        [TestMethod()]
        public void ControllaRispostaANF()
        {
            string risposta = null;
            using (System.IO.FileStream stm = new System.IO.FileStream(UtilityTest.basePath + "AnfResponse.xml", System.IO.FileMode.Open,
                System.IO.FileAccess.Read, System.IO.FileShare.None))
            {
                using (System.IO.StreamReader rdr = new System.IO.StreamReader(stm))
                {
                    risposta = rdr.ReadToEnd();
                }
            }
            string errori = string.Empty;
            GestioneFamiliari.ConsultazioneUnificataANF consultazione = null;
            GestioneFamiliari.ControllaRispostaANF(risposta, out consultazione, out errori);
        }
    }
}
