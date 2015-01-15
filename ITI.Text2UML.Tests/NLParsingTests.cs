using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ITI.Text2UML.Parsing.NaturalLanguage;

namespace ITI.Text2UML.Tests
{
    class NLParsingTests
    {
        [Test]
        public void BeStructure()
        {
            Assert.AreEqual(NLParser.Parse("A cat"), "class cat");
        }

        [Test]
        public void ClassParsingStructure()
        {
            Assert.AreEqual(NLParser.Parse("A class"), "class class");
        }

        [Test]
        public void AdjectiveStructure()
        {
            Assert.AreEqual(NLParser.Parse("A cat is blue"), "class cat thing1 blue");
        }

        [Test]
        public void DefinitionStructure()
        {
            Assert.AreEqual(NLParser.Parse("A blue, yellow and tiny cat and a stupid and big dog are weird and red animals and green pets"), "class cat thing1 blue thing2 yellow thing3 tiny class dog thing4 big thing5 weird class animal thing6 red class pet thing7 green cat -> animal dog -> animal cat -> pet dog -> pet");
        }

        [Test]
        public void TypeStructure()
        {
            Assert.AreEqual(NLParser.Parse("A blue cat. Blue is a color."),"class cat color blue");
        }

        [Test]
        public void ModalDefinitionStructure()
        {
            Assert.AreEqual(NLParser.Parse("Animals can be cats."), "class cat class animal cat -> animal");
        }

        [Test]
        public void PossessionStructure()
        {
            Assert.AreEqual(NLParser.Parse("A cat have a nose"), "class cat class nose cat --> nose");
            Assert.AreEqual(NLParser.Parse("A cat have five noses"), "class cat class nose cat --> nose (5 5)");
            Assert.AreEqual(NLParser.Parse("A cat have at least five noses"), "class cat class nose cat --> nose (5 n)");
            Assert.AreEqual(NLParser.Parse("A cat have less than five noses"), "class cat class nose cat --> nose (0 5)");
            Assert.AreEqual(NLParser.Parse("A cat have a nose. A cat have less than five noses."), "class cat class nose cat --> nose (1 5)");
            Assert.AreEqual(NLParser.Parse("A cat have less than five and more than two noses"), "class cat class nose cat --> nose (2 5)");
        }

        [Test]
        public void PositionStructure()
        {
            Assert.AreEqual(NLParser.Parse("A cat is in the house"), "class cat class house house --> cat");
            Assert.AreEqual(NLParser.Parse("Two cats are in the house"), "class cat class house house --> cat (2 2)");
            Assert.AreEqual(NLParser.Parse("At least two cats are in the house"), "class cat class house house --> cat (2 n)");
            Assert.AreEqual(NLParser.Parse("Less than two cats are in the house"), "class cat class house house --> cat (0 2)");
            Assert.AreEqual(NLParser.Parse("At least two but less than five cats are in the house"), "class cat class house house --> cat (2 5)");
        }
    }
}
