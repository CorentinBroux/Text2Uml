using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITI.Text2UML.Parsing.NaturalLanguage
{
    public static class NLGrammar
    {
        /// <summary>
        /// Lists all Penn Treebank P.O.S. Tags
        /// </summary>
        public static List<string> Keywords = new List<string>() { 
            "CC",   //  Coordinating conjunction
            "CD",   //	Cardinal number
            "DT",   //	Determiner
            "EX",   //	Existential there
            "FW",   //	Foreign word
            "IN",   //	Preposition or subordinating conjunction
            "JJ",   //	Adjective
            "JJR",  //	Adjective, comparative
            "JJS",  //	Adjective, superlative
            "LS",   //	List item marker
            "MD",   //	Modal
            "NN",   //	Noun, singular or mass
            "NNS",  //	Noun, plural
            "NNP",  //	Proper noun, singular
            "NNPS", //	Proper noun, plural
            "PDT",  //	Predeterminer
            "POS",  //	Possessive ending
            "PRP",  //	Personal pronoun
            "PRP$", //	Possessive pronoun
            "RB",   //	Adverb
            "RBR",  //	Adverb, comparative
            "RBS",  //	Adverb, superlative
            "RP",   //	Particle
            "SYM",  //	Symbol
            "TO",   //	to
            "UH",   //	Interjection
            "VB",   //	Verb, base form
            "VBD",  //	Verb, past tense
            "VBG",  //	Verb, gerund or present participle
            "VBN",  //	Verb, past participle
            "VBP",  //	Verb, non-3rd person singular present
            "VBZ",  //	Verb, 3rd person singular present
            "WDT",  //	Wh-determiner
            "WP",   //	Wh-pronoun
            "WP$",  //	Possessive wh-pronoun
            "WRB",  //	Wh-adverb
        };
        
        /// <summary>
        /// Lists all forms of the 'be' auxillary
        /// </summary>
        public static List<string> Verb_Be = new List<string>() { 
            "be",
            "am",
            "are",
            "is",
            "was",
            "were",
        };

        /// <summary>
        /// Lists all forms of the 'have' auxillary
        /// </summary>
        public static List<string> Verb_Have = new List<string>() {
            "have",
            "has",
            "had",
        };


        // Filled with user input
        public static List<Tuple<string, string>> Types;



    }
}
