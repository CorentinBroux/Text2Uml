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
                if (token != PCTokenType.Word && token != PCTokenType.EndOfInput && token != PCTokenType.Keyword)
                    token = PCTokenizer.GetNextToken();
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

                    if (t2 == PCTokenType.Word && t3 == PCTokenType.OpenPar) // If method (meaning v2 == v3)
                    {
                        Method m = new Method();
                        m.ParamTypes = new List<string>();
                        m.ReturnType = v1;
                        m.Name = v2;
                        token = PCTokenizer.GetNextToken();
                        while (token != PCTokenType.ClosePar)
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
                    else if (t2 == PCTokenType.Word) // If field or property
                    {
                        ITI.Text2UML.Model.Attribute att = new ITI.Text2UML.Model.Attribute(v1, v2);
                        boxes.Last().Attributes.Add(att);
                        token = t3;
                        continue; // t3 not used then dont call PCTokenizer.GetNextToken()
                    }
                    else if (t2 == PCTokenType.Link && t3 == PCTokenType.Word) // If link
                    {
                        Link link = new Link(v1, v3, Link.GetLinkTypeFromSymbol(v2));

                        token = PCTokenizer.GetNextToken();

                        if (token == PCTokenType.OpenPar) // if label
                        {
                            PCTokenType ta = PCTokenizer.GetNextToken();
                            string va = PCTokenizer.CurrentWordValue;
                            PCTokenType tb = PCTokenizer.GetNextToken();
                            string vb = PCTokenizer.CurrentWordValue;

                            if (ta == PCTokenType.Word && tb == PCTokenType.Word)
                            {
                                link.Label = String.Format("({0} {1})", va, vb);
                            }

                            //token = PCTokenizer.GetNextToken();
                        }

                        links.Add(link);
                    }



                }

            }



            // Return
            return new Tuple<List<Class>, List<Link>>(boxes, links);
        }

        /// <summary>
        /// Assign links to boxes.
        /// </summary>
        /// <param name="links">Links found in the pseudo code.</param>
        /// <param name="boxes">Boxes found in the pseudo code.</param>
        public static void AddLinksToBoxes(List<Link> links, List<Class> boxes)
        {
            List<Link> temp = links;
            List<Link> newLinks = new List<Link>();
            List<Link> deadLinks = new List<Link>();

            foreach (Link l in temp)
                foreach (Link l2 in links)
                {
                    if (l.From == l2.From && l.To == l2.To && l.Label != l2.Label)
                    {
                        int a1, b1, a2, b2; string min, max = "";
                        string[] str1 = l.Label.Split(new string[] { "(", " ", ")" }, StringSplitOptions.RemoveEmptyEntries);
                        string[] str2 = l2.Label.Split(new string[] { "(", " ", ")" }, StringSplitOptions.RemoveEmptyEntries);
                        Int32.TryParse(str1[0], out a1);
                        if (Int32.TryParse(str1[1], out a2) == false)
                            max += "n";
                        Int32.TryParse(str2[0], out b1);
                        if (Int32.TryParse(str2[1], out b2) == false)
                            max += "n";
                        min = Math.Max(a1, b1).ToString();
                        if (max == "")
                            max = Math.Min(a2, b2).ToString();
                        else if (max == "nn")
                            max = "n";
                        else
                            max = Math.Max(a2, b2).ToString(); ;
                        newLinks.Add(new Link(l.From, l.To, l.Type, String.Format("({0} {1})", min, max)));
                        deadLinks.Add(l);
                        deadLinks.Add(l2);
                    }
                }
            foreach (Link l in deadLinks)
                links.Remove(l);
            foreach (Link l in newLinks)
                links.Add(l);
            links = links.Distinct().ToList();
            

            foreach (Link link in links)
            {
                if (link.From != link.To)
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
                                        box.Linked = new List<Tuple<Class, LinkTypes, string>>();
                                    box.Linked.Add(new Tuple<Class, LinkTypes, string>(box2, link.Type, link.Label));
                                }

                            }
                            break;
                        }
                    }
            }
        }
    }

}
