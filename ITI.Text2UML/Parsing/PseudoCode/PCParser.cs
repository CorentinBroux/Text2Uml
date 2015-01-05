using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITI.Text2UML.Model;

namespace ITI.Text2UML.Parsing.PseudoCode
{
    public static class PCParser
    {
        /// <summary>
        /// Parse the pseudo code.
        /// </summary>
        /// <param name="input">Whole pseudo code</param>
        /// <returns>Returns Tuple<List<Class>, List<Link>> representing classes and links found in the pseudo code.</returns>
        public static Tuple<List<Class>, List<Link>> Parse(string input)
        {
            
            // Initialize PCTokenizer and lists
            PCTokenizer PCTokenizer = new PCTokenizer(input);
            List<Class> boxes = new List<Class>();
            List<Link> links = new List<Link>();
            List<string> boxNames = new List<string>(); // Not include "Unnamed" boxes

            PCTokenType token = PCTokenizer.GetNextToken();
            // Tokenize and parse
            while (PCTokenizer.CurrentToken != PCTokenType.EndOfInput)
            {
                token = PCTokenizer.CurrentToken;
                if (token == PCTokenType.Keyword)
                {
                    boxes.Add(new Class("Unnamed box"));
                    boxes.Last().Attributes = new List<ITI.Text2UML.Model.Attribute>();
                    boxes.Last().Methods = new List<Method>();
                    token = PCTokenizer.GetNextToken();
                    if (token == PCTokenType.Word)
                    {
                        boxes.Last().Name = PCTokenizer.CurrentWordValue;
                        
                        if (boxNames.Contains(PCTokenizer.CurrentWordValue))
                        {
                            boxes.Remove(boxes.Last());
                            Class item = boxes.FirstOrDefault(o => o.Name == PCTokenizer.CurrentWordValue);
                            boxes.Remove(item);
                            boxes.Add(item);
                        }
                        else
                        {
                            boxNames.Add(PCTokenizer.CurrentWordValue);
                        }
                    }
                        
                    
                    
                    token = PCTokenizer.GetNextToken();
                    continue;
                }
                
                if (token == PCTokenType.EndOfInput)
                    break;

                if (token == PCTokenType.Word)
                {
                    PCTokenType t1 = token;
                    string v1 = PCTokenizer.CurrentWordValue;
                    PCTokenType t2 = PCTokenizer.GetNextToken();
                    string v2 = PCTokenizer.CurrentWordValue;
                    PCTokenType t3 = PCTokenizer.GetNextToken();
                    string v3 = PCTokenizer.CurrentWordValue;

                    if(t2 == PCTokenType.Word && t3 == PCTokenType.OpenPar) // If method (meaning v2 == v3)
                    {
                        Method m = new Method();
                        m.ParamTypes = new List<string>();
                        m.ReturnType = v1;
                        m.Name = v2;
                        token = PCTokenizer.GetNextToken();
                        while(token != PCTokenType.ClosePar)
                        {
                            if (token == PCTokenType.EndOfInput)
                                throw new InvalidSyntaxException("Missing method closure");
                            else if (token == PCTokenType.Word)
                                m.ParamTypes.Add(PCTokenizer.CurrentWordValue);
                            else if (token != PCTokenType.ClosePar)
                                throw new InvalidSyntaxException("Invalid syntax");

                            token = PCTokenizer.GetNextToken();
                        }
                        boxes.Last().Methods.Add(m);
                        token = PCTokenizer.GetNextToken();
                    }
                    else if(t2 == PCTokenType.Word) // If field or property
                    {
                        ITI.Text2UML.Model.Attribute att = new ITI.Text2UML.Model.Attribute(v1, v2);
                        boxes.Last().Attributes.Add(att);
                        continue; // t3 not used then dont call PCTokenizer.GetNextToken()
                    }
                    else if(t2 == PCTokenType.Link && t3 == PCTokenType.Word) // If link
                    {
                        Link link = new Link(v1, v3, Link.GetLinkTypeFromSymbol(v2));
                        links.Add(link);
                        token = PCTokenizer.GetNextToken();
                    }


                   
                }
                    
            }
            
            
            
            // Return
            return new Tuple<List<Class>,List<Link>>(boxes, links);
        }

        /// <summary>
        /// Assign links to boxes.
        /// </summary>
        /// <param name="links">Links found in the pseudo code.</param>
        /// <param name="boxes">Boxes found in the pseudo code.</param>
        public static void AddLinksToBoxes(List<Link> links, List<Class> boxes)
        {
            foreach (Link link in links)
            {
                foreach (Class box in boxes)
                {
                    if (link.From == box.Name)
                    {
                        box.IsLinked = true;
                        foreach (Class box2 in boxes)
                        {
                            if (link.To == box2.Name)
                            {
                                if (box.Linked == null)
                                    box.Linked = new List<Tuple<Class, LinkTypes>>();
                                box.Linked.Add(new Tuple<Class, LinkTypes>(box2, link.Type));
                            }
                                
                        }
                        break;
                    }
                }
            }
        }
    }

}
