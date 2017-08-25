using Microsoft.VisualStudio.TestTools.UnitTesting;
using maskx.SystemWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Activities;

namespace maskx.SystemWorkflow.Tests
{
    [TestClass()]
    [TestCategory("ActivityFactory")]
    public class ActivityFactoryTests
    {
        [TestMethod()]
        [TestCategory("ActivityFactory/simple")]
        public void Create_String_True_Test()
        {
            var act1 = ActivityFactory.Create(
                "simple",
                (id) =>
                {
                    return Path.Combine(
                        Environment.CurrentDirectory,
                        "../../wf", id + ".xaml");
                },
                true);
            var act2 = ActivityFactory.Create(
               "simple",
               (id) =>
               {
                   return Path.Combine(
                       Environment.CurrentDirectory,
                       "../../wf", id + ".xaml");
               },
               true);
            Assert.AreNotEqual(act1, act2);
        }
        [TestMethod()]
        [TestCategory("ActivityFactory/simple")]
        public void Create_String_Fail_Test()
        {
            var act1 = ActivityFactory.Create(
                "simple",
                (id) =>
                {
                    return Path.Combine(
                        Environment.CurrentDirectory,
                        "../../wf", id + ".xaml");
                },
                false);
            var act2 = ActivityFactory.Create(
               "simple",
               (id) =>
               {
                   return Path.Combine(
                       Environment.CurrentDirectory,
                       "../../wf", id + ".xaml");
               },
               false);
            Assert.AreEqual(act1, act2);
        }
        [TestMethod()]
        [TestCategory("ActivityFactory/simple")]
        public void Create_Stream_True_Test()
        {
            var act1 = ActivityFactory.Create(
                "simple",
                (id) =>
                {
                    return File.OpenRead(Path.Combine(
                        Environment.CurrentDirectory,
                        "../../wf", id + ".xaml"));
                },
                true);
            var act2 = ActivityFactory.Create(
               "simple",
               (id) =>
               {
                   return File.OpenRead(Path.Combine(
                         Environment.CurrentDirectory,
                         "../../wf", id + ".xaml"));
               },
               true);
            Assert.AreNotEqual(act1, act2);
        }
        [TestMethod()]
        [TestCategory("ActivityFactory/simple")]
        public void Create_Stream_Fail_Test()
        {
            var act1 = ActivityFactory.Create(
                "simple",
                (id) =>
                {
                    return File.OpenRead(Path.Combine(
                        Environment.CurrentDirectory,
                        "../../wf", id + ".xaml"));
                },
                false);
            var act2 = ActivityFactory.Create(
               "simple",
               (id) =>
               {
                   return File.OpenRead(Path.Combine(
                         Environment.CurrentDirectory,
                         "../../wf", id + ".xaml"));
               },
               false);
            Assert.AreEqual(act1, act2);
        }
        [TestMethod]
        [TestCategory("ActivityFactory/expression")]
        public void ExpressionTest()
        {
            int a = 2;
            int b = 3;
            int c = 0;// c=(a+b)*100
            var act1 = ActivityFactory.Create(
                "expression",
                (id) =>
                {
                    return File.OpenRead(Path.Combine(
                        Environment.CurrentDirectory,
                        "../../wf", id + ".xaml"));
                });
            var rtv = WorkflowInvoker.Invoke(act1, new Dictionary<string, object>()
            {
                {"a",a },
                {"b",b }
            });
            c = (int)rtv["c"];
            Assert.AreEqual(c, (a + b) * 100);
        }
    }
}