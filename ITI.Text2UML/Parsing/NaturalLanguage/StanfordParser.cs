using java.io;
using edu.stanford.nlp.process;
using edu.stanford.nlp.ling;
using edu.stanford.nlp.trees;
using edu.stanford.nlp.parser.lexparser;
using Console = System.Console;
using System.IO;

namespace ITI.Text2UML.Parsing.NaturalLanguage
{
    public class StanfordParser
    {
        // Path to models extracted from `stanford-parser-3.5.0-models.jar`
        public static string jarRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\ThirdParty\stanford-parser-full-2014-10-31";
        public static string modelsDirectory = jarRoot + @"\edu\stanford\nlp\models";

        // Loading english PCFG parser from file
        public static LexicalizedParser lp = LexicalizedParser.loadModel(modelsDirectory + @"\lexparser\englishPCFG.ser.gz");

        /// <summary>
        /// Parse the sentence into a syntaxic tree.
        /// </summary>
        /// <param name="input">Sentence.</param>
        /// <returns>String representing the syntaxic tree.</returns>
        public static string Stanford_Parse(string input)
        {
            // This option shows loading and using an explicit tokenizer
            var sent2 = input;
            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var sent2Reader = new java.io.StringReader(sent2);
            var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
            sent2Reader.close();
            var tree = lp.apply(rawWords2);

            tokenizerFactory.setOptions("dcoref.singular");
            // Extract dependencies from lexical tree
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var gs = gsf.newGrammaticalStructure(tree);
            var tdl = gs.typedDependenciesCCprocessed();


#if DEBUG
            Tools.Tree t = NLParser.GetTree(tree.toString());
            Tools.Tree t2 = t.GetSubTree(1);
            System.Collections.Generic.List<Tools.Node> nodes = NLParser.GetTree(tree.toString()).Root.GetAllChildren();
#endif
            // Return tree expression
            return tree.toString();
        }
    }
}
