using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML
{
    public static class Parser
    {
        public static Tuple<List<ABox>, List<Link>> Parse(string input)
        {
            // Initialize tokenizer and lists
            Tokenizer tokenizer = new Tokenizer(input);
            List<ABox> boxes = new List<ABox>();
            List<Link> links = new List<Link>();

            TokenType token = tokenizer.GetNextToken();
            // Tokenize and parse
            while (tokenizer.CurrentToken != TokenType.EndOfInput)
            {
                token = tokenizer.CurrentToken;
                if (token == TokenType.Keyword)
                {
                    boxes.Add(new ABox());
                    boxes.Last().Attributes = new List<Attribute>();
                    boxes.Last().Methods = new List<Method>();
                    token = tokenizer.GetNextToken();
                    if (token == TokenType.Word)
                        boxes.Last().Name = tokenizer.WordValue;
                    else
                        boxes.Last().Name = "Unnamed box";
                        //throw new InvalidSyntaxException("Missing name");
                    
                    token = tokenizer.GetNextToken();
                    continue;
                }
                
                if (token == TokenType.EndOfInput)
                    break;

                if (token == TokenType.Word)
                {
                    TokenType t1 = token;
                    string v1 = tokenizer.WordValue;
                    TokenType t2 = tokenizer.GetNextToken();
                    string v2 = tokenizer.WordValue;
                    TokenType t3 = tokenizer.GetNextToken();
                    string v3 = tokenizer.WordValue;

                    if(t2 == TokenType.Word && t3 == TokenType.OpenPar) // If method (meaning v2 == v3)
                    {
                        Method m = new Method();
                        m.ParamTypes = new List<string>();
                        m.ReturnType = v1;
                        m.Name = v2;
                        token = tokenizer.GetNextToken();
                        while(token != TokenType.ClosePar)
                        {
                            if (token == TokenType.EndOfInput)
                                throw new InvalidSyntaxException("Missing method closure");
                            else if (token == TokenType.Word)
                                m.ParamTypes.Add(tokenizer.WordValue);
                            else if (token != TokenType.ClosePar)
                                throw new InvalidSyntaxException("Invalid syntax");

                            token = tokenizer.GetNextToken();
                        }
                        boxes.Last().Methods.Add(m);
                        token = tokenizer.GetNextToken();
                    }
                    else if(t2 == TokenType.Word) // If field or property
                    {
                        Text2UML.Attribute att = new Text2UML.Attribute(v1, v2);
                        boxes.Last().Attributes.Add(att);
                        continue; // t3 not used then dont call tokenizer.GetNextToken()
                    }
                    else if(t2 == TokenType.Link && t3 == TokenType.Word) // If link
                    {
                        Link link = new Link(v1, v3, Link.GetLinkTypeFromSymbol(v2));
                        links.Add(link);
                        token = tokenizer.GetNextToken();
                    }


                   
                }
                    
            }
            
            
            
            // Return
            return new Tuple<List<ABox>,List<Link>>(boxes, links);
        }
        public static void AddLinksToBoxes(List<Link> links, List<ABox> boxes)
        {
            foreach (Link link in links)
            {
                foreach (ABox box in boxes)
                {
                    if (link.From == box.Name)
                    {
                        box.IsLinked = true;
                        foreach (ABox box2 in boxes)
                        {
                            if (link.To == box2.Name)
                            {
                                if (box.Linked == null)
                                    box.Linked = new List<ABox>();
                                box.Linked.Add(box2);
                            }
                                
                        }
                        break;
                    }
                }
            }
        }
    }

    #region old
    //public class Parser
    //{
    //    /// <summary>
    //    /// Return text part concerning Aboxes (class, interface, abstract)
    //    /// </summary>
    //    /// <param name="text">input text to parse</param>
    //    /// <returns></returns>
    //    public static Tuple<List<ABox>,List<Link>> ExtractAboxes(string text)
    //    {
    //        Tokenizer tokenizer = new Tokenizer(text);
    //        Token token;
    //        List<ABox> ABoxes = new List<ABox>();
    //        List<Link> links = new List<Link>();

    //        do
    //        {
    //            token = tokenizer.GoToNextToken();
    //            if (token.IsKeyword)
    //            {
    //                ABox box =null;
    //                bool IsAbox=true;

    //                if(Enum.IsDefined(typeof(FirstLevelKeyword),token.Value))
    //                    box = new Class(tokenizer.GoToNextToken().Value, new List<Attribute>(), new List<Method>());
    //                //if (token.Value == "Class")
    //                //    box = new Class(tokenizer.GoToNextToken().Value, new List<Attribute>(), new List<Method>());
    //                //else if (token.Value == "Interface")
    //                //    box = new Interface(tokenizer.GoToNextToken().Value, new List<Attribute>(), new List<Method>());
    //                //else if (token.Value == "Abstract")
    //                //    box = new AbstractClass(tokenizer.GoToNextToken().Value, new List<Attribute>(), new List<Method>());
    //                else if (token.Value == "Links")
    //                    IsAbox=false;
    //                else
    //                    throw new InvalidSyntaxException("Error : " + token.Value + " is not a valid first level keyword.");

    //                // Go to first delimiter
    //                do
    //                {
    //                    token = tokenizer.GoToNextToken();
    //                } while (token.IsDelimiter);

    //                string stringData = "";

    //                // Get the data written between the delimiters
    //                while (token.Type != TokenType.Delimiter)
    //                {
    //                    stringData += token.Value + " ";
    //                    token = tokenizer.GoToNextToken();
    //                }

    //                if (IsAbox)
    //                    ABoxes.Add(FillAboxFromDataString(box, stringData));
    //                else
    //                    links = Links(stringData);
    //            }

    //        } while (token.Type != TokenType.EoF);

    //        return Tuple.Create(ABoxes,links);
    //    }

    //    private static ABox FillAboxFromDataString(ABox aBox, string dataString)
    //    {



    //        Tokenizer tokenizer = new Tokenizer(dataString);
    //        Token token;
    //        bool isAttributesCurrentSection = true;
    //        do
    //        {
    //            token = tokenizer.GoToNextToken();
    //            if (token.IsEoL)
    //            {
    //                continue;
    //            }
    //            else if (token.IsKeyword)
    //            {
    //                if (token.Value == "Attributes")
    //                    isAttributesCurrentSection = true;
    //                else if (token.Value == "Methods")
    //                    isAttributesCurrentSection = false;
    //                else
    //                    throw new InvalidSyntaxException("Error : " + token.Value + " is not a valid second level keyword.");
    //            }
    //            else if (token.IsOther)
    //            {
    //                if (isAttributesCurrentSection)
    //                    aBox.Attributes.Add(new Attribute(token.Value, tokenizer.GoToNextToken().Value));
    //                else
    //                {
    //                    Method m = new Method();
    //                    m.ReturnType = token.Value;
    //                    string str ="";
    //                    while(!token.Value.Contains(")"))
    //                    {
    //                        token = tokenizer.GoToNextToken();
    //                        str += token.Value;
    //                    }

    //                    List<string> data = ExtractMethodData(str);
    //                    m.Name = data[0];
    //                    data.RemoveAt(0);
    //                    m.ParamTypes = data;
    //                    aBox.Methods.Add(m);
    //                }
    //            }

    //        } while (token.Type != TokenType.EoF);



    //        return aBox;
    //    }

    //    private static List<Link> Links(string dataString)
    //    {
    //        Tokenizer tokenizer = new Tokenizer(dataString);
    //        Token token;
            
    //        bool newLine = true;
    //        List<Link> links = new List<Link>();
        
    //        Link link = new Link(null, null, LinkTypes.Extends);


    //        do
    //        {
 
    //            token = tokenizer.GoToNextToken();
    //            if (token.IsEoL)
    //            {
    //                newLine=true;
    //                continue;
    //            }
    //            else if (token.IsOther)
    //            {
    //                if(newLine)
    //                {
    //                    link = new Link(token.Value, null, LinkTypes.Extends);
    //                    newLine = false;
    //                }
    //                else
    //                {
    //                    link.To = token.Value;
    //                    links.Add(link);
    //                    newLine = true;
    //                }
    //            }
    //            else if(token.IsLinkSymbol)
    //            {
    //                link.Type = Link.GetLinkTypeFromSymbol(token.Value);
    //            }

                

    //        } while (token.Type != TokenType.EoF);

    //        return links;
    //    }


    //    private static List<string> ExtractMethodData(string dataString)
    //    {

    //        List<string> list = new List<string>();
    //        string[] strings = dataString.Split(new char[] {'(',')'}, StringSplitOptions.RemoveEmptyEntries); // name      type1, type2, type3
    //        list.Add(strings[0]); // Add method name
    //        if (strings.Length == 1)
    //            return list;
    //        strings = strings[1].Split(new string[]{", "},StringSplitOptions.RemoveEmptyEntries); // type1      type2      type3
    //        foreach (string s in strings)
    //            list.Add(s); // Add parameter types
    //        return list;
    //    }


    //    /// <summary>
    //    /// Report dead links and remove them  from the 'links' list
    //    /// </summary>
    //    /// <param name="links"></param>
    //    /// <param name="boxes"></param>
    //    public static void ReportDeadLinks(List<Link> links, List<ABox> boxes)
    //    {
    //        List<string> boxNames = new List<string>();
    //        foreach (ABox box in boxes)
    //        {
    //            boxNames.Add(box.Name);
    //        }
    //        List<Link> deadLinks = new List<Link>();
    //        foreach (Link link in links)
    //        {
    //            if (!(boxNames.Contains(link.From) && boxNames.Contains(link.To)))
    //            {
    //                deadLinks.Add(link);
    //            }
                    
    //        }
    //        links = links.Except(deadLinks).ToList();
    //        if(deadLinks.Count==0)
    //            return;
    //        string msg = "WARNING !\n\nDead links reported :\n";
    //        foreach(Link link in deadLinks)
    //            msg+="\t"+link.From+" --> "+link.To+"\n";
    //        //System.Windows.Forms.MessageBox.Show(msg);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <remarks>Better use after cleaning the 'links' list with the 'ReportDeadLinks' method</remarks>
    //    /// <param name="links"></param>
    //    /// <param name="boxes"></param>
    //    
    //}
    #endregion
}
