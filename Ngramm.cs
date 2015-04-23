using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using QuickGraph;

namespace AbrakadabraLib
{
    public class Ngramm
    {
        private List<Ngramm> components;//составные части может тогда заменить тип на Unigramm??...
        private List<Ngramm> parents;

        public int Counter { get; set; }

        public List<Ngramm> Components { get { return components; } set { components = value; } }

        public List<Ngramm> Parents { get { return parents; } set { parents = value; } }

        public List<Ngramm> Childs { get; set; }

        public string ID { get { return ToString(); } }

        public Ngramm()
        {
            components = new List<Ngramm>();
            parents = new List<Ngramm>();
            Childs = new List<Ngramm>();
            Counter = 1;
        }

        public Ngramm(List<Ngramm> comp, List<Ngramm> par)
        {
            components = comp;
            parents = par;
            //обратная связь
            foreach (Ngramm parent in par)
                parent.Childs.Add(this);
            Childs = new List<Ngramm>();
            Counter = 1;
        }

        public override bool Equals(object obj)
        {
            Ngramm second = obj as Ngramm;
            if (second == null)
                return false;
            if (this.Components.Count != second.Components.Count)
                return false;

            if (this.Components.Count == 0)
            {
                if (this.ID.Equals(second.ID))
                    return true;
                else
                    return false;
            }
            //для каждого элемента первого должно найтись соответствие в элементах второго
            foreach (Ngramm a in this.Components)
            {
                bool findedAnalog = false;
                foreach (Ngramm b in second.Components)
                    if ((a as Unigramm).LexicalBase.Content.Equals((b as Unigramm).LexicalBase.Content))
                    {
                        findedAnalog = true;
                        break;
                    }
                if (!findedAnalog)
                    return false;
            }
            return true;
        }

        //не подходит для юниграмм!!
        public bool Equals(List<Ngramm> secondsComp)
        {
            if (this.Components.Count != secondsComp.Count)
                return false;
            //для каждого элемента первого должно найтись соответствие в элементах второго
            foreach (Ngramm a in this.Components)
            {
                bool findedAnalog = false;
                foreach (Ngramm b in secondsComp)
                    if ((a as Unigramm).LexicalBase.Content.Equals((b as Unigramm).LexicalBase.Content))
                    {
                        findedAnalog = true;
                        break;
                    }
                if (!findedAnalog)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return recurToString(this);            
        }

        private string recurToString(Ngramm gramm)
        {
            if (gramm.Components.Count != 0)
            {
                String res = "";
                foreach (Ngramm c in gramm.Components)
                    res+=recurToString(c)+" ";
                return res;
            }
            else
                return (gramm as Unigramm).LexicalBase.Content;
        }
    }
}
