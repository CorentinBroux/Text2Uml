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
    class NLParsingTests
    {
        [Test]
        public void BeStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat"))), PCFormatter.Format("class cat"));
        }

        [Test]
        public void ClassParsingStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A class"))), PCFormatter.Format("class class"));
        }

        [Test]
        public void AdjectiveStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat is blue"))), PCFormatter.Format("class cat thing1 blue"));
        }

        [Test]
        public void DefinitionStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A blue, yellow and tiny cat and a stupid and big dog are weird and red animals and green pets"))), PCFormatter.Format("class cat thing1 blue thing2 yellow thing3 tiny class dog thing4 big thing5 weird class animal thing6 red class pet thing7 green cat -> animal dog -> animal cat -> pet dog -> pet"));
        }

        [Test]
        public void TypeStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A blue cat. Blue is a color."))),PCFormatter.Format("class cat color blue"));
        }

        [Test]
        public void ModalDefinitionStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("Animals can be cats."))), PCFormatter.Format("class cat class animal cat -> animal"));
        }

        [Test]
        public void PossessionStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have a nose"))), PCFormatter.Format("class cat class nose cat --> nose"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have five noses"))), PCFormatter.Format("class cat class nose cat --> nose (5 5)"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have at least five noses"))), PCFormatter.Format("class cat class nose cat --> nose (5 n)"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have less than five noses"))), PCFormatter.Format("class cat class nose cat --> nose (0 5)"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have a nose. A cat have less than five noses."))), PCFormatter.Format("class cat class nose cat --> nose (1 5)"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat have less than five and more than two noses"))), PCFormatter.Format("class cat class nose cat --> nose (2 5)"));
        }

        [Test]
        public void PositionStructure()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A cat is in the house"))), PCFormatter.Format("class cat class house house --> cat"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("Two cats are in the house"))), PCFormatter.Format("class cat class house house --> cat (2 2)"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("At least two cats are in the house"))), PCFormatter.Format("class cat class house house --> cat (2 n)"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("Less than two cats are in the house"))), PCFormatter.Format("class cat class house house --> cat (0 2)"));
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("At least two but less than five cats are in the house"))), PCFormatter.Format("class cat class house house --> cat (2 5)"));
        }

        [Test]
        public void ComplexSentence()
        {
            Assert.AreEqual(PCFormatter.Format(NLParser.Parse(StanfordParser.Stanford_Parse("A tiny blue dog is a weird animal and a cute pet and a talented musician is a good artist."))), PCFormatter.Format("class dog thing1 tiny thing2 blue class animal thing3 weird class pet thing4 cute class musician thing5 talented class artist thing6 good dog -> animal dog -> pet musician -> artist"));
        }
    }
}
