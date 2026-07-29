using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using INPS.DNA.Security.Idm;
using INPS.DNA.Security.Roles;
using INPS.DNA.Context;
using INPS.Pensioni.Liquidazione.ServiceTest.SvrLiquidazione;


namespace INPS.Pensioni.Liquidazione.UnitTest
{
    /// <summary>
    /// Summary description for ServiceLiquidazioneTest
    /// </summary>
    [TestClass]
    public class ServiceDecodificaTest
    {
        public ServiceDecodificaTest()
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
            //IdmIdentity ident;

            //ident = new IdmIdentity("4444444444", "ABBCDA74A10H501B", "Mario", "Rossi", "mrossi", @"domain\mrossi", "mrossi@inps.it", "0600", "060000");

            //System.Threading.Thread.CurrentPrincipal = new IdmPrincipal(ident, "cn=INPS.Pensioni.Liquidazione:oper:060000,dc=inps,dc=it;cn=INPS.Pensioni.Liquidazione:oper:700500,dc=inps,dc=it;");

            //INPS.DNA.Context.OperationContextInfo.CreateUnitTestContext();
            //INPS.DNA.Context.OperationContextInfo.Current.CurrentOffice = OfficeList.Offices["060000"];


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
        public void TestGetDecodifica()
        {
            ServiceTest.SvrLiquidazione.DecodificaClient objWS = new DecodificaClient();
            ServiceTest.SvrLiquidazione.AreaDecodifica decodifica = objWS.GetDecodifica();
            Assert.IsTrue(
                decodifica != null &&  
                decodifica.ElencoStatiCivili != null && 
                decodifica.ElencoStatiEsteri != null &&
                decodifica.ElencoConiugeOFiglio != null &&
                decodifica.ElencoDetrazioniReddito != null &&
                decodifica.ElencoTutore != null &&
                decodifica.ElencoDelegato != null &&
                decodifica.ElencoModalitaPagamento != null &&
                decodifica.ElencoTipoPagamento != null &&
                decodifica.ElencoProvince != null &&
                decodifica.ElencoCodiceCristallizzazione != null &&
                decodifica.ElencoTipoPensione != null &&
                decodifica.ElencoCodiceAzienda != null &&
                decodifica.ElencoGradoInvalidita != null &&
                decodifica.ElencoProrataEnel != null &&
                decodifica.ElencoComunicazioneCampi1_2 != null &&
                decodifica.ElencoComunicazioneCampo3 != null &&
                decodifica.ElencoComunicazioneCampo4 != null &&
                decodifica.ElencoCodiciNatura != null &&
				decodifica.ElencoCategoriePensione != null &&
				decodifica.ElencoFondiPensione != null &&
				decodifica.ElencoStatiPensione != null
                , "Area decodifica non valorizzata correttamente");
			Assert.IsTrue(decodifica.ElencoCategoriePensione.Count<ServiceTest.SvrLiquidazione.AreaDecodifica.DatiCategoriaPensione>() == 96, "Numero errato di categorie pensione");
			Assert.IsTrue(decodifica.ElencoFondiPensione.Count<ServiceTest.SvrLiquidazione.AreaDecodifica.DatiFondoPensione>() == 13, "Numero errato di fondi pensione");
			Assert.IsTrue(decodifica.ElencoStatiPensione.Count<ServiceTest.SvrLiquidazione.AreaDecodifica.DatiStatoPensione>() > 0, "Numero errato di stati pensione");
        }

        [TestMethod]
        public void TestGetComuniPerProvinciaSrv()
        {
            ServiceTest.SvrLiquidazione.DecodificaClient objWS = new DecodificaClient();
            ServiceTest.SvrLiquidazione.AreaDecodifica.DatiComune[] elencoComuni = objWS.GetComuniPerProvincia("NA");
            Assert.IsTrue(elencoComuni != null && elencoComuni.Length > 0, "Elenco comuni non valorizzato correttamente");
            
            elencoComuni = objWS.GetComuniPerProvincia("AA");
            Assert.IsTrue(elencoComuni == null, "Elenco comuni non nullo");

        }
    }
}

