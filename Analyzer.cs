using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbrakadabraLib
{
    public struct TextObject
    {
        public List<WordDictionary> Phrases;
        public WordDictionary Dictionary;

        public TextObject(List<WordDictionary> phrases,WordDictionary dictionary)
        {
            Phrases = phrases;
            Dictionary = dictionary;
        }
    }
    public static class Analyzer
    {
        public static WordDictionary GlobalDictionary = new WordDictionary();
        public static List<List<WordDictionary>> Texts = new List<List<WordDictionary>>();
        public static List<WordDictionary> TextDictionarys = new List<WordDictionary>();


        public static void FillDictionary(string path)
        {
            string[] allFiles = System.IO.Directory.GetFiles(path, "*.txt");

            for (int i = 0; i < allFiles.Length; i++)
            {
                char[] text = System.IO.File.ReadAllText(allFiles[i], System.Text.Encoding.GetEncoding(1251)).ToCharArray();
                TextObject textObject = ParseText(text);
                //добавление триграмм и биграмм в общий словарь
                GlobalDictionary.CopyNgramms(textObject.Phrases);

                Texts.Add(textObject.Phrases);
                TextDictionarys.Add(textObject.Dictionary);
            }
        }

        public static TextObject ParseText(char[] text)
        {
            String txt = new String(text);
            WordDictionary textDictionary = new WordDictionary(txt);
            //извлечение терминов(в последствии и словоформ) из конкретного текста
            List<WordDictionary> phrases = new List<WordDictionary>(); //предложения текста
            WordDictionary currentPhrase = new WordDictionary(); //вспомогательный (по-другому?), хранятся компоненты текущего предложения
            for (int j = 0, k = 0, p=0; j < text.Length; )
            {
                switch (text[j])
                {
                    case '"'://кавычки
                        text[j] = ' ';//на кавычки пока никак не реагируем
                        break;
                    case ' '://пробел
                        if (j != k)
                        {
                            char[] tmp = new char[j - k];
                            Array.Copy(text, k, tmp, 0, (j - k));
                            GlobalDictionary.addTermAndLexicalForm(new String(tmp));
                            textDictionary.addTermAndLexicalForm(new String(tmp));
                            currentPhrase.addTermAndLexicalForm(new String(tmp));
                            k = j;
                        }
                        j++;
                        k++;
                        break;
                    case '.'://точка
                        if (j != k)
                        {
                            char[] tmp = new char[j - k];
                            Array.Copy(text, k, tmp, 0, (j - k));
                            GlobalDictionary.addTermAndLexicalForm(new String(tmp));
                            textDictionary.addTermAndLexicalForm(new String(tmp));
                            currentPhrase.addTermAndLexicalForm(new String(tmp));
                            k = j;
                        }
                        char[] tmpp = new char[j - p];
                        Array.Copy(text,p,tmpp,0,(j-p));
                        currentPhrase.Text = new String(tmpp);
                        phrases.Add(currentPhrase);
                        currentPhrase = new WordDictionary();

                        j++;
                        k++;
                        p = j;
                        break;
                    default:
                        j++;
                        break;
                }
                if ((j == text.Length) && (j != p) && (currentPhrase.LexicalForms.Count!=0))
                {
                    char[] tmpp = new char[j - p];
                    Array.Copy(text, p, tmpp, 0, (j - p));
                    currentPhrase.Text = new String(tmpp);
                    phrases.Add(currentPhrase);
                }
            }
            //создание словарей для каждой фразы
            parsePhrases(phrases);
            //добавление триграмм и биграмм в словарь конкретного текста
            textDictionary.CopyNgramms(phrases);
            TextObject textObject = new TextObject(phrases, textDictionary);
            return textObject;
            
        }

        public static List<double> countTFIDF(List<WordDictionary> phrases,WordDictionary textDictionary )
        {
            List<double> summs= new List<double>();
            for(int i=0; i<phrases.Count;i++)
            {
                summs.Add(0);
                foreach (Unigramm lex in phrases[i].LexicalForms)
                {
                    Unigramm lexGlobal = GlobalDictionary.LexicalForms.Find(p=>p.ID.Equals(lex.ID));
                    Unigramm lexText = textDictionary.LexicalForms.Find(p => p.ID.Equals(lex.ID));
                    double tf = ((double)lexText.Counter / (double)textDictionary.N);
                    double idf = Math.Log((double)GlobalDictionary.N / (double)lexGlobal.Counter);
                    summs[i] += tf * idf;
                }

                foreach (Ngramm bigramm in phrases[i].Bigramms)
                {
                    Ngramm bigrammGlobal = GlobalDictionary.Bigramms.Find(p => p.Equals(bigramm));
                    Ngramm bigrammText = textDictionary.Bigramms.Find(p => p.Equals(bigramm));
                    double tf = ((double)bigrammText.Counter / (double)textDictionary.N);
                    double idf = Math.Log((double)GlobalDictionary.N / (double)bigrammGlobal.Counter);
                    summs[i] += tf * idf;
                }
                foreach (Ngramm threegramm in phrases[i].Threegramms)
                {
                    Ngramm threegrammGlobal = GlobalDictionary.Threegramms.Find(p => p.Equals(threegramm));
                    Ngramm threegrammText = textDictionary.Threegramms.Find(p => p.Equals(threegramm));
                    double tf = ((double)threegrammText.Counter / (double)textDictionary.N);
                    double idf = Math.Log((double)GlobalDictionary.N / (double)threegrammGlobal.Counter);
                    summs[i] += tf * idf;
                }
            }
            return summs;
        }

        public static List<WordDictionary> Summirize(List<WordDictionary> phrases, List<double> veights, int count)
        {
            List<WordDictionary> newPhrases = new List<WordDictionary>();
            newPhrases.AddRange(phrases);
            for (int i = 0; i < (phrases.Count - count); i++)
            {
                double min = veights[0];
                int minIndex = 0;
                for (int j = 0; j < newPhrases.Count; j++)
                    if (veights[j] < min)
                    {
                        min = veights[j];
                        minIndex = j;
                    }
                newPhrases.RemoveAt(minIndex);
            }
            return newPhrases;
        }


        public static void parsePhrases(List<WordDictionary> phrases)
        {
            foreach (WordDictionary phrase in phrases)
            {
                //извлечение всех комбинаторно возможных биграмм
                for (int i = 0;i < phrase.LexicalForms.Count; i++)
                {
                    for (int j = i + 1; j < phrase.LexicalForms.Count; j++)
                    {
                        List<Ngramm> comp = new List<Ngramm>();
                        comp.Add(phrase.LexicalForms[i]);
                        comp.Add(phrase.LexicalForms[j]);
                        Ngramm newBigramm = new Ngramm(comp, comp); //что за херня скомпонентами??Что они вообще такое????   
                        phrase.Bigramms.Add(newBigramm);
                    }
                }

                //извлечение всех комбинаторно возможных триграмм
                for (int i = 0; i < phrase.LexicalForms.Count; i++)
                {
                    for (int j = i + 1; j < phrase.LexicalForms.Count; j++)
                    {
                        for (int k = j + 1; k < phrase.LexicalForms.Count; k++)
                        {
                            List<Ngramm> comp = new List<Ngramm>();
                            comp.Add(phrase.LexicalForms[i]);
                            comp.Add(phrase.LexicalForms[j]);
                            comp.Add(phrase.LexicalForms[k]);
                            List<Ngramm> par = new List<Ngramm>();
                            par.Add(phrase.LexicalForms[i].Childs.Find(p => p.Parents.Contains(phrase.LexicalForms[j])));
                            Ngramm t1 = phrase.LexicalForms[i];
                            Ngramm t2 = phrase.LexicalForms[k];
                            Ngramm t3 = t1.Childs.Find(p => p.Parents.Contains(t2));
                            par.Add(t3);
                            par.Add(phrase.LexicalForms[j].Childs.Find(p => p.Parents.Contains(phrase.LexicalForms[k])));
                            Ngramm newThreegramm = new Ngramm(comp, par);
                            phrase.Threegramms.Add(newThreegramm);
                        }
                    }                    
                }
            }
        }


        //добавляем  в граф
        //graph.AddVertex(newForm);
        //graph.AddEdge(new Relation(newBigramm, newBigramm.Components[0]));



    }
}
