using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExoParseV2.theUniverse;
using ExoParseV2.elements;
using System.Security.Cryptography.X509Certificates;
using ExoParseV2;
using ExoParseV2.universe;
using ExoParseV2.utilities;

namespace ExoParseV2_test
{

    [TestClass]
    public class ExpressionTesting
    {
        UniverseFactory uf;
        [TestInitialize]
        public void Init()
        {
            uf = new UniverseFactory();

        }
        [TestMethod]
        public void SimpleLiteral()
        {
            Universe vers = uf.CreateUniverse();
            Assert.AreEqual(vers.ParseLine("3.2")?.Execute(), 3.2);
        }

        [TestMethod]
        public void SimpleExpression()
        {
            Universe vers = uf.CreateUniverse();
            Assert.AreEqual(vers.ParseLine("3 + 2")?.Execute(), 5);
        }

        [TestMethod]
        public void VariableValue()
        {
            var vers = uf.CreateUniverse();

            var exp1 = vers.ParseLine("a = 42");
            var exp2 = vers.ParseLine("b = true");
            exp1.Execute();
            exp2.Execute();
            var a = vers.NamedItems["a"];
            var b = vers.NamedItems["b"];

            Assert.AreEqual(a.Execute(), 42);
            Assert.AreEqual(b.Execute(), LogicUtils.True_double);
        }

        [TestMethod]
        public void SetDefinition()
        {
            Universe vers = uf.CreateUniverse();

            var exp1 = vers.ParseLine("a = 42");
            var exp2 = vers.ParseLine("ppa := ++a");
            var exp3 = vers.ParseLine("c := b := ppa");

            exp1.Execute();
            exp2.Execute();
            exp3.Execute();
            var a = vers.NamedItems["a"];
            var b = vers.NamedItems["b"];
            var c = vers.NamedItems["c"];
            var ppa = vers.NamedItems["ppa"];

            // a should still be 42, ppa or ++a should not have been executed.
            Assert.AreEqual(a.Execute(), 42);

            // The definition of both b and c should be ppa
            Assert.AreEqual(b.Definition, ppa);
            Assert.AreEqual(c.Definition, ppa);
            //

            //Executing ppa should execute its definition which is ++a, thus a should be 43 afterwards:
            ppa.Execute();
            Assert.AreEqual(a.Execute(), 43);
            //

            b.Execute();
            Assert.AreEqual(a.Execute(), 44);
            c.Execute();
            Assert.AreEqual(a.Execute(), 45);
        }
    }
}
