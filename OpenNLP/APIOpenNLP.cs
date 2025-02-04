﻿using System.IO;

/*      Useful cheatsheet for NLP part-of-speech acronyms.

        CC Coordinating conjunction
        CD Cardinal number
        DT Determiner
        EX Existential there
        FW Foreign word
        IN Preposition or subordinating conjunction
        JJ Adjective
        JJR Adjective, comparative
        JJS Adjective, superlative
        LS List item marker
        MD Modal
        NN Noun, singular or mass
        NNS Noun, plural
        NNP Proper noun, singular
        NNPS Proper noun, plural
        PDT Predeterminer
        POS Possessive ending
        PRP Personal pronoun
        PRP$ Possessive pronoun
        RB Adverb
        RBR Adverb, comparative
        RBS Adverb, superlative
        RP Particle
        SYM Symbol
        TO to
        UH Interjection
        VB Verb, base form
        VBD Verb, past tense
        VBG Verb, gerund or present participle
        VBN Verb, past participle
        VBP Verb, non­3rd person singular present
        VBZ Verb, 3rd person singular present
        WDT Wh­determiner
        WP Wh­pronoun
        WP$ Possessive wh­pronoun
        WRB Wh­adverb 
*/

namespace OpenNLP
{
    public static class APIOpenNLP
    {
        private static Tools.SentenceDetect.MaximumEntropySentenceDetector mSentenceDetector;
        private static Tools.Tokenize.EnglishMaximumEntropyTokenizer mTokenizer;
        private static Tools.PosTagger.EnglishMaximumEntropyPosTagger mPosTagger;
        private static Tools.Chunker.EnglishTreebankChunker mChunker;
        private static Tools.SentimentAnalysis.AFINNLexicon mAFINNLexicon;
        private static Tools.NameFind.EnglishNameFinder mNameFinder;

        public static string[] SplitSentences(string paragraph)
        {
            if (mSentenceDetector == null)
            {
                mSentenceDetector = new Tools.SentenceDetect.EnglishMaximumEntropySentenceDetector("../../../SharpNLP Training Data/EnglishSD.nbin");
            }

            return mSentenceDetector.SentenceDetect(paragraph);
        }

        public static string[] TokenizeSentence(string sentence)
        {
            if (mTokenizer == null)
            {
                mTokenizer = new Tools.Tokenize.EnglishMaximumEntropyTokenizer("../../../SharpNLP Training Data/EnglishTok.nbin");
            }

            return mTokenizer.Tokenize(sentence);
        }

        public static string[] PosTagTokens(string[] tokens)
        {
            if (mPosTagger == null)
            {
                mPosTagger = new Tools.PosTagger.EnglishMaximumEntropyPosTagger("../../../SharpNLP Training Data/EnglishPOS.nbin");
            }

            return mPosTagger.Tag(tokens);
        }

        public static string ChunkTokensPostag(string[] tokens, string[] postags)
        {
            if (mChunker == null)
            {
                mChunker = new Tools.Chunker.EnglishTreebankChunker("../../../SharpNLP Training Data/EnglishChunk.nbin");
            }

            return mChunker.GetChunks(tokens, postags);
        }

        public static int AFINNAnalysis(string[] sentence)
        {
            if (mChunker == null)
            {
                mAFINNLexicon = new Tools.SentimentAnalysis.AFINNLexicon("../../../AFINN Lexicon/AFINN.txt");
            }

            return mAFINNLexicon.SentimentallyTagSentence(sentence);
        }

        public static SharpEntropy.GisModel TrainLanguageModel(string trainingDataFile)
        {
            System.IO.StreamReader trainingStreamReader = new System.IO.StreamReader(trainingDataFile);
            SharpEntropy.ITrainingEventReader eventReader = new SharpEntropy.BasicEventReader(new SharpEntropy.PlainTextByLineDataReader(trainingStreamReader));
            SharpEntropy.GisTrainer trainer = new SharpEntropy.GisTrainer();
            trainer.TrainModel(eventReader);
            return new SharpEntropy.GisModel(trainer);
        }

        public static void POSTagger_Method(string sent)
        {
            File.WriteAllText("POSTagged.txt", sent + "\n\n");
            string[] split_sentences = SplitSentences(sent);
            foreach (string sentence in split_sentences)
            {
                File.AppendAllText("POSTagged.txt", sentence + "\n");
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);
                string chunkPostag = ChunkTokensPostag(tokens, tags);

                for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                {
                    File.AppendAllText("POSTagged.txt", tokens[currentTag] + " - " + tags[currentTag] + "\n\n");
                }

                File.AppendAllText("POSTagged.txt", chunkPostag);
                File.AppendAllText("POSTagged.txt", "\n\n");
            }
        }

        public static string FindNames(string sentence)
        {
            if (mNameFinder == null)
            {
                mNameFinder = new Tools.NameFind.EnglishNameFinder("../../../SharpNLP Training Data/NameFind/");
            }

            var models = new[] { "date", "location", "money", "organization", "percentage", "person", "time" };
            return mNameFinder.GetNames(models, sentence);
        }
    }
}
