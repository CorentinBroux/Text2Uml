using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2UML.Model
{
    class Parser
    {
        /// <summary>
        /// Return text part concerning Aboxes (class, interface, abstract)
        /// </summary>
        /// <param name="text">input text to parse</param>
        /// <returns></returns>
        public static List<ABox> ExtractAboxes(string text)
        {
            Tokenizer tokenizer = new Tokenizer(text);
            Token token;
            List<ABox> ABoxes = new List<ABox>();

            do
            {
                token = tokenizer.GoToNextToken();
                if (token.IsKeyword)
                {
                    ABox box =null;
                    bool IsAbox=true;

                    if (token.Value == "$Class")
                        box = new Class(tokenizer.GoToNextToken().Value, new List<Attribute>(), new List<Method>());
                    else if (token.Value == "$Interface")
                        box = new Interface(tokenizer.GoToNextToken().Value, new List<Attribute>(), new List<Method>());
                    else if (token.Value == "$Abstract")
                        box = new AbstractClass(tokenizer.GoToNextToken().Value, new List<Attribute>(), new List<Method>());
                    else if (token.Value == "$Links")
                        IsAbox=false;
                    else
                        throw new InvalidSyntaxException("Error : " + token.Value + " is not a valid first level keyword.");

                    // Go to first delimiter
                    do
                    {
                        token = tokenizer.GoToNextToken();
                    } while (token.IsDelimiter);

                    string stringData = "";

                    // Get the data written between the delimiters
                    while (token.Type != TokenType.Delimiter)
                    {
                        stringData += token.Value + " ";
                        token = tokenizer.GoToNextToken();
                    }

                    if (IsAbox)
                        ABoxes.Add(FillAboxFromDataString(box, stringData));
                    else
                        Links(stringData);
                }
                //else
                //    throw new InvalidSyntaxException("Error : " + token.Value + " is not a keyword.");

            } while (token.Type != TokenType.EoF);

            return ABoxes;
        }

        private static ABox FillAboxFromDataString(ABox aBox, string dataString)
        {



            Tokenizer tokenizer = new Tokenizer(dataString);
            Token token;
            bool isAttributesCurrentSection = true;
            do
            {
                token = tokenizer.GoToNextToken();
                if (token.IsEoL)
                {
                    continue;
                }
                else if (token.IsKeyword)
                {
                    if (token.Value == "$Attributes")
                        isAttributesCurrentSection = true;
                    else if (token.Value == "$Methods")
                        isAttributesCurrentSection = false;
                    else
                        throw new InvalidSyntaxException("Error : " + token.Value + " is not a valid second level keyword.");
                }
                else if (token.IsOther)
                {
                    if (isAttributesCurrentSection)
                        aBox.Attributes.Add(new Attribute(token.Value, tokenizer.GoToNextToken().Value));
                    else
                    {
                        Method m = new Method();
                        m.ReturnType = token.Value;
                        List<string> data = ExtractMethodData(tokenizer.GoToNextToken().Value);
                        m.Name = data[0];
                        data.RemoveAt(0);
                        m.ParamTypes = new List<string>();
                        foreach (string s in data)
                        {
                            m.ParamTypes.Add(s);
                        }
                        aBox.Methods.Add(m);
                    }
                }

            } while (token.Type != TokenType.EoF);



            return aBox;
        }

        private static void Links(string dataString)
        {
            Tokenizer tokenizer = new Tokenizer(dataString);
            Token token;
            
            bool newLine = true;
            List<Link> links = new List<Link>();

            string debug = ""; // DEBUG
            
            
            Link link = new Link(null, null, LinkTypes.Extends);


            do
            {
 
                token = tokenizer.GoToNextToken();
                if (token.IsEoL)
                {
                    newLine=true;
                    continue;
                }
                else if (token.IsOther)
                {
                    if(newLine)
                    {
                        link = new Link(token.Value, null, LinkTypes.Extends);
                        newLine = false;
                    }
                    else
                    {
                        link.To = token.Value;
                        links.Add(link);
                    }
                }
                else if(token.IsLinkSymbol)
                {
                    link.Type = Link.GetLinkTypeFromSymbol(token.Value);
                }

                

            } while (token.Type != TokenType.EoF);


            // DEBUG
            foreach (Link l in links)
            {
                debug += "Link\n" + l.From + " " + l.Type + " " + l.To + "\n\n";
            }
            System.Windows.MessageBox.Show(debug);
        }


        private static List<string> ExtractMethodData(string dataString)
        {
            return new List<string>(){"string 1", "string 2", "string 3", "string 4"};
        }
    }
}
