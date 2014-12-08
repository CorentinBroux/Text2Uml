using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Tests
{
    public class Class1
    {

        [Test]
        public void appending_something()
        {
            //Arrange
            StringBuilder b = new StringBuilder();

            //Act
            b.AppendFormat("A{0}B{1}C{2}", "a", "b", "c");
            string res = b.ToString();

            //assert
            Assert.AreEqual("AaBbc", res);



        }

    }
}
