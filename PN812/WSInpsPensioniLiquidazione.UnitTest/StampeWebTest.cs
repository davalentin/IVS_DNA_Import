using System;
using System.IO;
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


    /// <summary>
    ///This is a test class for StampeWebTest and is intended
    ///to contain all StampeWebTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StampeWebTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
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

        /// <summary>
        ///A test for GetStampaDomanda
        ///</summary>
        [TestMethod()]
        public void TestGetStampaDomanda()
        {
            //VEL calcolata definitiva
            long numeroDomanda = 2038563700014;
            byte[] datiStampa = null;
            string errori = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);
            bool result = GestioneStampeWeb.GetStampaDomanda(datiPensione, out datiStampa, out errori);
            Assert.IsFalse(!result);

            Stream readStream = new MemoryStream(datiStampa);

            string saveTo = @"C:\Users\cozzolinop\Desktop\TP150_" + numeroDomanda.ToString() + ".pdf";
            // create a write stream
            FileStream writeStream = new FileStream(saveTo, FileMode.Create, FileAccess.Write);
            // write to the stream
            ReadWriteStream(readStream, writeStream);

            System.Text.Encoding ascii = System.Text.Encoding.UTF8;
            char[] asciiChars = new char[ascii.GetCharCount(datiStampa, 0, (datiStampa).Length)];
            ascii.GetChars((byte[])datiStampa, 0, (datiStampa).Length, asciiChars, 0);
            string row_ = new string(asciiChars);
            row_ = row_.Replace('\r', ' ');
            return;
        }

        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }

        [TestMethod]
        public void TestIsDomandaConStampa()
        {
            long numeroDomanda = 2008832500008;
            string errori = string.Empty;
            GestionePensione.DatiPensione datiPensione = null;
            GestionePensione.GetPensioneByNumeroDomandaAndProg(numeroDomanda, null, out datiPensione);

            bool btest = GestioneStampeWeb.IsDomandaConStampaGenerata(datiPensione, out errori);
        }
    }
}

