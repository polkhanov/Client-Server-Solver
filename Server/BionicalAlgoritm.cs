using System;
using System.Collections.Generic;
using System.Linq;



namespace GenAlg
{
    class GenPoint
    {
        public double x1;
        public double x2;

        public double y;

        public GenPoint(double x1, double x2, double y)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y = y;
        }

    }

    class Bionical
    {
        double[,] table;
        Random random = new Random();
        int x1_start = 0;
        int x1_finish = 455;
        int x2_start = 0;
        int x2_finish = 105;

        int defaultCountChild;
        int defaultCountGeneration;

        List<GenPoint> childs = new List<GenPoint>();
        List<GenPoint> parents = new List<GenPoint>();

        public Bionical(double[,] table, int countChild,int countGen)
        {

            this.table = table; defaultCountChild = countChild;defaultCountGeneration = countGen;
            if (countGen > 90) x1_start = 400; x2_start = 90;
        }

        public double f(double x1, double x2)
        {
            return table[4,0] * x1 + table[4,1] * x2;

        }
        public bool valid(double x1, double x2)
        {
            return (table[0,0] * x1 + table[0,1] * x2 <= table[0,2]) && (table[1,0] * x1 + table[1,1] * x2 <=  table[1,2]) && (table[2,0] * x1 + table[2,1]*x2 <= table[2,2]);
        }

        public List<GenPoint> getChild(GenPoint parent, int countChild, int gen)
        {
            List<GenPoint> childs = new List<GenPoint>(countChild);
            int h = int.MaxValue / (gen + 1);
            while (childs.Count != countChild)
            {
                double x1 = random.Next(x1_start, x1_finish);
                double x2 = random.Next(x2_start, x2_finish);
                if (!valid(x1, x2) || (x1 == parent.x1 && x2 == parent.x2))
                {
                    continue;
                }
                childs.Add(new GenPoint(x1, x2, f(x1, x2)));
            }
            return childs;
        }

        public List<GenPoint> getBestPoints(List<GenPoint> points, int count)
        {
            List<GenPoint> sorted = points.OrderBy(o => o.y).ToList();
            sorted.Reverse();
            return sorted.GetRange(0, count);
        }


        public List<GenPoint> getX()
        {
                       
            while (parents.Count != defaultCountChild)
            {
                double x1 = random.Next(x1_start, x1_finish);
                double x2 = random.Next(x2_start, x2_finish);
                if (!valid(x1, x2))
                {
                    continue;
                }
                parents.Add(new GenPoint(x1, x2, f(x1, x2)));
            }


            for (int gen = 0; gen < defaultCountGeneration; gen++)
            {
                
                foreach (GenPoint parent in parents)
                {
                   childs.AddRange(getChild(parent, defaultCountChild, gen));
                }

                parents = getBestPoints(childs, defaultCountChild);
            }
            return parents;
        }

    }

    
}
