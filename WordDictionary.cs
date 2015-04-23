using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbrakadabraLib
{
    public class WordDictionary
    {
        List<Term> terms; //термины (начальная форма слова и статистика по ней)
        List<Unigramm> lexicalForms; //словоформы
        List<Ngramm> bigramms; //биграммы
        List<Ngramm> threegramms; //триграммы

        public int N {get; set;} //число предложений

        //NgrammsGraph graph; //граф 

        public String Text { get; set; }
        
        public WordDictionary()
        {
            terms = new List<Term>();
            lexicalForms = new List<Unigramm>();
            bigramms = new List<Ngramm>();
            threegramms = new List<Ngramm>();
            //graph = new NgrammsGraph(true);
            N = 0;
            Text = "";
        }

        public WordDictionary(String txt)
        {
            terms = new List<Term>();
            lexicalForms = new List<Unigramm>();
            bigramms = new List<Ngramm>();
            threegramms = new List<Ngramm>();
            //graph = new NgrammsGraph(true);
            N = 0;
            Text = txt;
        }

        public NgrammsGraph Graph { get { return DrawGraph(); } }

        public List<Term> Terms {get { return terms; }}

        public List<Unigramm> LexicalForms {get { return lexicalForms; }}

        public List<Ngramm> Bigramms {get { return bigramms; }}

        public List<Ngramm> Threegramms {get { return threegramms; } }

        public void addTermAndLexicalForm(String content)
        {
            //добавляем новый термин в словарь(без проверки словоформ пока)
            Term newTerm = new Term(content);
            if (!existTerm(newTerm))
                addTerm(newTerm);
            //добавляем новую лексическую форму
            Unigramm newForm = new Unigramm(newTerm);
            if (!existLexicalForm(newForm))
                lexicalForms.Add(newForm);
            else
            {
                lexicalForms.Find(p => p.ID.Equals(newForm.ID)).Counter++;
            }
        }

        public void CopyNgramms(List<WordDictionary> phrases)
        {
            foreach (WordDictionary phrase in phrases)
            {
                foreach (Ngramm gramm in phrase.Bigramms)
                    if (!existBigramm(gramm))
                    {
                        List<Ngramm> components = new List<Ngramm>();
                        foreach(Ngramm comp in gramm.Components)
                            components.Add(lexicalForms.Find(p => p.ID.Equals((comp as Unigramm).ID)));
                        Ngramm newBigramm = new Ngramm(components, components);
                        Bigramms.Add(newBigramm);
                    }
                    else
                        Bigramms.Find(p => p.Equals(gramm.Components)).Counter++;

                foreach (Ngramm gramm in phrase.Threegramms)
                    if (!existThreegramm(gramm))
                    {
                        List<Ngramm> components = new List<Ngramm>();
                        foreach (Ngramm comp in gramm.Components)
                            components.Add(lexicalForms.Find(p => p.ID.Equals((comp as Unigramm).ID)));

                        List<Ngramm> parents = new List<Ngramm>();
                        foreach (Ngramm par in gramm.Parents)
                            parents.Add(Bigramms.Find(p => p.Equals(par.Components)));
                        Ngramm newThreegramm = new Ngramm(components, parents);
                        Threegramms.Add(newThreegramm);
                    }
                    else
                        Threegramms.Find(p => p.Equals(gramm.Components)).Counter++;
                N++;
            }
        }


        public bool existLexicalForm(Ngramm gramm)
        {
            return lexicalForms.Exists(p => p.ID.Equals(gramm.ID));
        }

        //проверка, нет ли уже такой биграммы
        public bool existBigramm(Ngramm gramm)
        {
            return bigramms.Exists(p => p.Equals(gramm.Components));
        }
   

        //проверка, нет ли уже такой триграммы
        public bool existThreegramm(Ngramm gramm)
        {
            return threegramms.Exists(p => p.Equals(gramm));
        }

        private bool existTerm(Term term)
        {
            return terms.Exists(p=>p.Content.Equals(term.Content)
                                      );
        }

        public void addTerm(Term newTerm)
        {
            terms.Add(newTerm);
        }



        public void addNgramm(Ngramm gramm)
        {
            switch (gramm.Components.Count)
            {
                case 2:
                    break;
                case 3:
                    break;
            }
        }


        public NgrammsGraph DrawGraph()
        {
            NgrammsGraph graph = new NgrammsGraph(true);
            foreach (Ngramm gr in bigramms)
                graph.AddVertex(gr);
            foreach (Ngramm gr in threegramms)
                graph.AddVertex(gr);
            foreach (Unigramm uni in lexicalForms)
            {
                graph.AddVertex(uni);
                foreach (Ngramm gr in uni.Childs)
                    graph.AddEdge(new Relation(uni,gr));
            }
            foreach (Ngramm gr in bigramms)
                foreach(Ngramm ch in gr.Childs)
                    graph.AddEdge(new Relation(gr, ch));
            return graph;
        }

        //в Спарте этот метод скинули бы сгоры. Подумай над этим.
        public NgrammsGraph DrawGraph(List<WordDictionary> phrases)
        {
            NgrammsGraph graph = new NgrammsGraph(true);
            foreach (Ngramm gr in bigramms)
            {
                bool flag = false;
                foreach (WordDictionary phrase in phrases)
                    if (phrase.existBigramm(gr))
                        flag = true;
                if (flag)
                    graph.AddVertex(gr);
            }
            foreach (Ngramm gr in threegramms)
            {
                bool flag = false;
                foreach (WordDictionary phrase in phrases)
                    if (phrase.existThreegramm(gr))
                        flag = true;
                if (flag)
                   graph.AddVertex(gr);
            }
            foreach (Unigramm uni in lexicalForms)
            {
                bool flag = false;
                foreach (WordDictionary phrase in phrases)
                    if (phrase.existLexicalForm(uni))
                        flag = true;
                if (flag)
                {
                    graph.AddVertex(uni);
                    foreach (Ngramm gr in uni.Childs)
                    {
                        flag = false;
                        foreach (WordDictionary phrase in phrases)
                            if (phrase.existBigramm(gr))
                                flag = true;
                        if (flag)
                          graph.AddEdge(new Relation(uni, gr));
                    }
                }
            }
            foreach (Ngramm gr in bigramms)
            {
                bool flag = false;
                foreach (WordDictionary phrase in phrases)
                    if (phrase.existBigramm(gr))
                        flag = true;
                if (flag)
                    foreach (Ngramm ch in gr.Childs)
                    {
                        flag = false;
                        foreach (WordDictionary phrase in phrases)
                            if (phrase.existThreegramm(ch))
                                flag = true;
                        if (flag)
                           graph.AddEdge(new Relation(gr, ch));
                    }
            }
            return graph;
        }

    }
}
