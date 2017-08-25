using maskx.SystemWorkflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Xml.Linq;

namespace UnitTest
{
    /// <summary>
    /// Summary description for JsonObject
    /// </summary>
    [TestClass]
    [TestCategory("WFJSON")]
    public class WFJsonTest
    {

        class MyClass
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        dynamic obj = new
        {
            a = 123,
            c = new
            {
                a = 1,
                b = 2
            },
            b = new[] {
                new {b1=1,b2=2 },
                new {b1=3,b2=4 }
            }
        };

        #region Constructor
        [TestMethod]
        [TestCategory("WFJSON/Constructor")]
        public void Constructor_New_Test()
        {
            WFJson j = null;
            j = new WFJson();
            Assert.AreEqual(j.ToString(), "{}", "create a empty JsonObject fail");
        }
        [TestMethod]
        [TestCategory("WFJSON/Constructor")]
        public void Constructor_JObject_Test()
        {
            var jobj = new JObject(new JProperty("a", 1));
            var j = new WFJson(jobj);
            Assert.AreEqual(j.ToString(), "{\"a\":1}", "create a JosnObject with JObject fail");

        }
        [TestMethod]
        [TestCategory("WFJSON/Constructor")]
        public void Constructor_JArray_Test()
        {
            var jArr = new JArray(1, 2);
            var j = new WFJson(jArr);
            Assert.AreEqual(j.ToString(), "[1,2]");
        }
        [TestMethod]
        [TestCategory("WFJSON/Constructor")]
        public void Constructor_String_Test()
        {
            string str = "123";
            var j = new WFJson(str);
            Assert.AreEqual(j.V<string>(), str);
        }
        [TestMethod]
        [TestCategory("WFJSON/Constructor")]
        public void Constructor_Class_Test()
        {
            MyClass my = new MyClass() { Name = "Name", Age = 23 };
            var j = new WFJson(my);
            var rtv = j.V<MyClass>();
            Assert.IsInstanceOfType(rtv, typeof(MyClass));
            Assert.AreEqual(rtv.Name, my.Name);
            Assert.AreEqual(rtv.Age, my.Age);
        }
        #endregion

        #region Method
        [TestMethod]
        [TestCategory("WFJSON/Method")]
        public void Parse_Test()
        {
            var b = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var j = WFJson.Parse(b);
            Assert.IsInstanceOfType(j, typeof(WFJson));
        }
        [TestMethod]
        [TestCategory("WFJSON/Method")]
        public void V_Test()
        {
            var b = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var j = WFJson.Parse(b);
            Assert.AreEqual<int>(j["a"].V<int>(), 123);
        }
        [TestMethod]
        [TestCategory("WFJSON/Method")]
        public void V_Path_Test()
        {
            var b = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var j = WFJson.Parse(b);
            Assert.AreEqual<int>(j.V<int>("b[1].b1"), 3);
        }
        #endregion

        #region Indexer
        [TestMethod]
        [TestCategory("WFJSON/Indexer")]
        public void Indexer_Int_Get_Test()
        {
            var b = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var j = WFJson.Parse(b);
            Assert.AreEqual<int>(j[0].V<int>(), 123);
        }
        [TestMethod]
        [TestCategory("WFJSON/Indexer")]
        public void Indexer_Int_Set_Test()
        {
            var j = new WFJson
            {
                ["a.b[0].c"] = 2
            };
            Assert.IsNotNull(j[0]);//a
            Assert.IsNotNull(j[0][0]);//a.b
            Assert.IsNotNull(j[0][0][0]);//a.b[0]
            Assert.IsNotNull(j[0][0][0][0]);//a.b[0].c
            Assert.AreEqual(j[0][0][0][0].V<int>(), 2);
        }
        [TestMethod]
        [TestCategory("WFJSON/Indexer")]
        public void Indexer_String_Get_Test()
        {
            var b = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var j = WFJson.Parse(b);
            Assert.AreEqual<int>(j["a"].V<int>(), 123);
        }
        [TestMethod]
        [TestCategory("WFJSON/Indexer")]
        public void Indexer_String_Set_Test()
        {
            var j = new WFJson
            {
                ["a.b[0].c"] = "a"
            };
            Assert.IsNotNull(j["a"]);
            Assert.IsNotNull(j["a.b"]);
            Assert.IsNotNull(j["a.b[0]"]);
            Assert.IsNotNull(j["a.b[0].c"]);
            Assert.AreEqual(j["a.b[0].c"].V<string>(), "a");
        }
        [TestMethod]
        [TestCategory("WFJSON/Indexer")]
        public void Indexer_String_Get_Object_Test()
        {
            MyClass my = new MyClass() { Name = "Name", Age = 23 };
            var j = WFJson.Parse("{a:null}");
            j["a"] = new WFJson(my);
            var rtv = j["a"].V<MyClass>();
            Assert.IsInstanceOfType(rtv, typeof(MyClass));
            Assert.AreEqual(rtv.Name, my.Name);
            Assert.AreEqual(rtv.Age, my.Age);
        }

        #endregion

        #region IList Add
        [TestMethod]
        [TestCategory("WFJSON/ILIst/Add")]
        public void Add_Int_Test()
        {
            var j = WFJson.Parse("[]");
            j.Add(1);
            Assert.AreEqual(j.ToString(), "[1]");
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst/Add")]
        public void Add_Bool_Test()
        {
            var j = WFJson.Parse("[]");
            j.Add(true);
            Assert.AreEqual(j.ToString(), "[true]");
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst/Add")]
        public void Add_DateTime_Test()
        {
            var j = WFJson.Parse("[]");
            DateTime dt = DateTime.Now;
            j.Add(dt);
            Assert.AreEqual(j[0].V<DateTime>(), dt);
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst/Add")]
        public void Add_String_Test()
        {
            var j = WFJson.Parse("[]");
            j.Add("a");
            Assert.AreEqual(j.ToString(), "[\"a\"]");
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst/Add")]
        public void Add_Object_Test()
        {
            var j = WFJson.Parse("[]");
            j.Add(obj);
            Assert.AreEqual(j.ToString(), "[" + new WFJson(obj) + "]");
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst/Add")]
        public void Add_WFJson_Test()
        {
            var j = WFJson.Parse("[]");
            var a = new WFJson("sd");
            j.Add(a);
            Assert.AreEqual(j[0].ToString(), a.ToString());
        }
        #endregion

        #region IList 
        [TestMethod]
        [TestCategory("WFJSON/ILIst")]
        public void GetEnumeratorTest()
        {
            var j = new WFJson(obj);
            foreach (var item in j)
            {
                Assert.IsNotNull(item);
            }
            foreach (var item in j["b"])
            {
                Assert.IsNotNull(item);
            }
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst")]
        public void Contains_Int_Test()
        {
            var j = WFJson.Parse("[1,2,3]");
            Assert.IsTrue(j.Contains(1));
            Assert.IsFalse(j.Contains(5));
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst")]
        public void Contains_WFJson_Test()
        {
            var j = WFJson.Parse("[]");
            var a = new WFJson(1);
            Assert.IsFalse(j.Contains(a));
            j.Add(a);
            Assert.IsTrue(j.Contains(a));
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst")]
        public void IndexOf_WFJson_Test()
        {
            var j = WFJson.Parse("[]");
            var a = new WFJson(1);
            Assert.AreEqual<int>(j.IndexOf(a), -1);
            j.Add(a);
            Assert.AreEqual<int>(j.IndexOf(a), 0);
        }
        [TestMethod]
        [TestCategory("WFJSON/ILIst")]
        public void IndexOf_Int_Test()
        {
            var j = WFJson.Parse("[]");
            Assert.AreEqual(j.IndexOf(1), -1);
            j.Add(1);
            Assert.AreEqual(j.IndexOf(1), 0);
        }
        #endregion

        #region Property

        #endregion

    }
}
