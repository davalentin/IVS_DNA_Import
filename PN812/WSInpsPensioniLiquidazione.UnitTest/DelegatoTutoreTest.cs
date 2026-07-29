using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using INPS.DNA.Security.Idm;
using INPS.DNA.Context;
using INPS.DNA.Security.Roles;

namespace INPS.Pensioni.Liquidazione.UnitTest
{
	/// <summary>
	/// Summary description for DelegatoTutoreTest
	/// </summary>
	[TestClass]
	public class DelegatoTutoreTest
	{
		public DelegatoTutoreTest()
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

        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion Additional test attributes

        [TestMethod]
		public void TestMethod1()
		{
			//AreaRispostaRiepilogo.DatiRiepilogoAnagrafica datiRiepilogoAnagrafica
		}
	}
}
