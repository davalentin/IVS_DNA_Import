using INPS.Pensioni.Liquidazione.BLCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace INPS.Pensioni.Liquidazione.ServiceTest
{
    
    
    /// <summary>
    ///This is a test class for UtilityTest and is intended
    ///to contain all UtilityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilityTest
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


        /// <summary>
        ///A test for PulisciOggetto
        ///</summary>
        [TestMethod()]
        public void PulisciOggettoTest()
        {
            Class1 class1 = new Class1() { C1Prop1 = "AA\0\0\0\0", C1Prop2 = "\0\0\0" };
            object obj = class1;          
            //Utility.PulisciOggetto(obj);            
        }
    }

    public class Class1
    {
        public string C1Prop1 { get; set; }
        public string C1Prop2 { get; set; }

        public Class2 Class2 { get; set; }

        public Class1()
        {
            Class2 = new Class2() { C2prop1 = "\0\0\0", C2Prop2 = "\0\0\0" };
        }
    }

    public class Class2
    {
        public string C2prop1 { get; set; }
        public string C2Prop2 { get; set; }
        public Class3 Class3 { get; set; }

        public Class2()
        {
            Class3 = new Class3() { C3Prop3 = "\0\0\0" };
        }
    }

    public class Class3
    {
        public int C3Prop1 { get; set; }
        public int C3Prop2 { get; set; }
        public string C3Prop3 { get; set; }
        public List<Class4> Class4List { get; set; }

        public Class3()
        {
            Class4List = new List<Class4>();
            Class4List.Add(new Class4() { C4Prop2 = "\0\0\0\0", C4Prop3 = "\0\0\0\0" });
            Class4List.Add(new Class4() { C4Prop2 = "\0\0\0"});
        }
    }

    public class Class4
    {
        public long C4Prop1 { get; set; }
        public string C4Prop2 { get; set; }
        public string C4Prop3 { get; set; }

        public List<List<Class5>> Class5 { get; set; }

        public Class4()
        {
            Class5 = new List<List<Class5>>();
            Class5.Add(new List<Class5>());
            Class5.Add(new List<Class5>());

            Class5[0].Add(new Class5() { C5Prop1 = "\0\0\0\0", C5Prop2 = "\0\0" });
            Class5[1].Add(new Class5() { C5Prop1 = "\0\0\0\0", C5Prop3 = 12});

        }
    }

    public class Class5
    {
        public string C5Prop1 { get; set; }
        public string C5Prop2 { get; set; }
        public int C5Prop3 { get; set; }
    }
}
