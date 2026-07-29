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
    /// Summary description for UniDetraTest
    /// </summary>
    [TestClass]
    public class UniDetraTest
    {
        public UniDetraTest()
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
            //INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.Offices["060000"];
        }

        [TestMethod]
        public void TestMethod1()
        {
            ServiceReferences.UniDetra.DetrazioniClient proxy = new ServiceReferences.UniDetra.DetrazioniClient();

            ServiceReferences.UniDetra.RichiestaRicercaDetrazione richiesta = new ServiceReferences.UniDetra.RichiestaRicercaDetrazione();
            richiesta.CodiceFiscale = "BNDLSS79H11H501V";
            richiesta.Decorrenza = 2017;
            richiesta.Sicurezza = new ServiceReferences.UniDetra.Sicurezza();
            //richiesta.Sicurezza.Username = "AIODMN64T26L805R";
            richiesta.Sicurezza.CsAppName = "PN812_InpsPensioneLiquidazione";
            richiesta.Sicurezza.CsAppKey = "PN812_InpsPensioneLiquidazione";
            
            ServiceReferences.UniDetra.EsitoRicercaDetrazione risposta = proxy.Ricerca(richiesta);

        }

        [TestMethod]
        public void GetDetrazioniByDatiPensione()
        {
            GestioneDetrazioni.RispostaDetrazioni risposta = null;
            string errori = string.Empty;

            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(2038734900003, null, out datiPensione);

            GestioneDetrazioni.GetDetrazioniByDatiPensione(datiPensione, "CSRPMR51M49C621B", false, 0, out risposta, out errori);
        }

        [TestMethod]
        public void SerializeResponse()
        {
            SrvUniDetra.RicercaResponse response = (SrvUniDetra.RicercaResponse)UtilityTest.Deserialize_Input(UtilityTest.basePath + "UniDetraResponse.xml", new SrvUniDetra.RicercaResponse());
        }
    }
}
