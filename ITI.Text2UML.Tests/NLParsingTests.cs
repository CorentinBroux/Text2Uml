using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ITI.Text2UML.Parsing.NaturalLanguage;
using Text2UML.View;

namespace ITI.Text2UML.Tests
{
    [TestFixture]
    class NLParsingTests
    {

        [SetUp]
        public void InitializeTests()
        {
            NLParser.j = 1;
        }


        [Test]
        public void BeStructure()
        {

            String expect = PCFormatter.Format("class dog");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A dog"))));
        }

        [Test]
        public void ClassParsingStructure()
        {
            String expect = PCFormatter.Format("class class");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A class"))));
        }

        [Test]
        public void AdjectiveStructure()
        {
            String expect = PCFormatter.Format("class cat thing1 blue");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat is blue"))));
        }

        [Test]
        public void DefinitionStructure()
        {
            String expect = PCFormatter.Format("class cat thing1 blue thing2 yellow thing3 tiny class dog thing4 stupid thing5 big class animal thing6 weird class pet thing7 green cat -> animal cat -> pet dog -> animal dog -> pet ");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A blue, yellow and tiny cat and a stupid and big dog are weird animals and green pets"))));
        }

        [Test]
        public void ModalDefinitionStructure()
        {
            String expect = PCFormatter.Format("class cat class animal cat -> animal");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("Animals can be cats."))));
        }

        [Test]
        public void PossessionStructure_have()
        {
            String expect = PCFormatter.Format("class cat class foot cat --> foot (1 1)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have a foot"))));
        }

        [Test]
        public void PossessionStructure_have_number()
        {
            String expect = PCFormatter.Format("class cat class foot cat --> foot (5 5)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have five feet"))));
        }

        [Test]
        public void PossessionStructure_have_least()
        {
            String expect = PCFormatter.Format("class foot class cat cat --> foot (5 n)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have at least five feet"))));
        }

        [Test]
        public void PossessionStructure_have_less()
        {
            String expect = PCFormatter.Format("class cat class foot cat --> foot (0 5)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have less than five feet"))));
        }

        [Test]
        public void PossessionStructure_have_less_and_more()
        {
            String expect = PCFormatter.Format("class cat class foot cat --> foot (2 5)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have less than five and more than two feet"))));
        }

        [Test]
        public void PositionStructure_is()
        {
            String expect = PCFormatter.Format("class house class cat house --> cat ");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat is in the house"))));
        }

        [Test]
        public void PositionStructure_number_is()
        {
            String expect = PCFormatter.Format("class house class cat house --> cat (2 2)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("Two cats are in the house"))));
        }

        [Test]
        public void PositionStructure_least_is()
        {
            String expect = PCFormatter.Format("class house class cat house --> cat (2 n)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("At least two cats are in the house"))));
        }

        [Test]
        public void PositionStructure_less_is()
        {
            String expect = PCFormatter.Format("class house class cat house --> cat (0 2)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("less than two cats are in the house"))));
        }

        [Test]
        public void PositionStructure_least_and_less_is()
        {
            String expect = PCFormatter.Format("class house class cat house --> cat (2 5)");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("At least two but less than five cats are in the house"))));
        }

        [Test]
        public void ComplexSentence()
        {
            String expect = PCFormatter.Format("class dog thing1 tiny thing2 blue class animal thing3 weird class pet thing4 cute class musician thing5 talented class artist thing6 good dog -> animal dog -> pet musician -> artist");
            Assert.AreEqual(expect, PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A tiny blue dog is a weird animal and a cute pet and a talented musician is a good artist."))));
        }
    }
}
