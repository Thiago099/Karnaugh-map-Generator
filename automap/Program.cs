using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace automap
{
    class Program
    {
        void swap<T>(List<T> l,int a,int b)
        {
            T temp = l[a];
            l[a] = l[b];
            l[b] = temp;
        }
        List<string> generate_map(int c)
        {
            var p = 2;//number of values
            List<string> lines = new List<string>();
            for (int i = 0; i < Math.Pow(p, c); i++)
            {
                var sb = new StringBuilder();
                for (int k = 1; k < c + 1; k++)
                    sb.Append((int)(i / Math.Pow(p, k - 1) % p));
                lines.Add(sb.ToString());
            }
            for (int j = 3; j < Math.Pow(p, c); j = j * 2 + 1)
            {
                for (int i = j; i < lines.Count; i += j)
                {
                    swap(lines, i, i - 1);
                }
            }
            return lines;
            
        }
        public Program()
        {
           
            int[,] v ={
                {0,0,0,1},
                {0,0,1,1},
                {0,1,0,0},
                {0,1,1,1},
                {1,0,1,1},
                {1,0,0,0},
                {1,1,0,0},
                {1,1,1,1},
            };
            var r = v.GetLength(1) - 1;
            var l1 = r / 2;
            var l2 = r - l1;


            List<string> x = generate_map(l1);
            List<string> y = generate_map(l2);
            var map = new int[x.Count, y.Count];

            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    for (int k = 0; k < v.GetLength(0); k++)
                    {
                        bool found = true;
                        int m = 0;
                        for (int l = 0; l < x[i].Length; l++)
                        {
                            if(x[i][l]-'0'!=v[k,m])
                            {
                                found = false;
                                break;
                            }
                            m++;
                        }
                        if (!found) continue;
                        for (int l = 0; l < y[j].Length; l++)
                        {
                            if ((y[j][l]-'0') != v[k, m])
                            {
                                found = false;
                                break;
                            }
                            m++;
                        }
                        if (!found) continue;
                        
                        map[i, j] = v[k, v.GetLength(1) - 1];
                    }
                }
            }
            int n = 0;
            var sb = new StringBuilder();
            for (int i = 0; i < l1; i++)
            {
                sb.Append((char)(n + 'A'));
                n++;
            }
            section.xv = sb.ToString();
            ConsoleColor.Cyan.Write(sb.ToString());
            sb = new StringBuilder();

            Console.Write("\\");
            for (int i = 0; i < l2; i++)
            {
                sb.Append((char)(n + 'A'));
                n++;
            }
            section.yv = sb.ToString();
            ConsoleColor.Cyan.Write(sb.ToString());

            Console.Write("\t");
            for (int i = 0; i < y.Count; i++)
            {
                ConsoleColor.Yellow.Write($"{y[i]}\t");
            }
            Console.WriteLine();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                ConsoleColor.Yellow.Write($"{x[i]}\t");
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    ConsoleColor.White.Write($"{map[i, j]}\t");
                }
                Console.WriteLine();
            }
            List<section> groups = new List<section>();
            section.max_x = x.Count;
            section.max_y = y.Count;
            section.map = map;
            section.x = x;
            section.y = y;
            section.needed = new bool[x.Count, y.Count];
            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    if (map[i, j] == 1) section.needed[i,j] = true;
                    else section.needed[i,j] = false;
                }
            }
            for (int i = 0; i < x.Count; i++)
            {
                for (int j = 0; j < y.Count; j++)
                {
                    if(map[i,j]==1)
                    {
                        var c = new section(i, j, i, j);
                        groups.Add(c);
                    }
                }
            }
            groups = section.expand(groups);
            for (int i = 0; i < groups.Count; )
            {
                if (groups[i].size % 2 == 1&& groups[i].size!=1) groups.RemoveAt(i);
                else i++;
            }
            sb = new StringBuilder();

            while (true)
            {
                int max = groups[0].size;
                section maxs = groups[0];
                for (int i = 1; i < groups.Count; i++)
                {
                    if (groups[i].size > max)
                    {
                        max = groups[i].size;
                        maxs = groups[i];
                    }
                }
                Console.WriteLine($"{maxs.y1}.{maxs.x1},{maxs.y2}.{maxs.x2}");
                sb.Append(maxs.result()+"+");
                maxs.set_used();
                groups.Remove(maxs);
                for (int i = 0; i < groups.Count;)
                {
                    if (!groups[i].isneeded())
                    {
                        groups.RemoveAt(i);
                    }
                    else
                        i++;
                }
                if (groups.Count == 0) break;
            }
            ConsoleColor.Green.WriteLine(sb.ToString(0,sb.Length-1));

        }

        static void Main(string[] args)
        {
            new Program();
            Console.ForegroundColor = ConsoleColor.Black;
        }
    }
    class section
    {
        public static int max_x;
        public static int max_y;
        public static int[,] map;
        public static bool[,] needed;
        public static List<string> x;
        public static List<string> y;
        public static string xv;
        public static string yv;
        public int height
            => y1 == y2 ? 1 : y2 > y1 ? y2 - y1 + 1 : max_y - y1 + y2+1;
        public int width
            => x1 == x2 ? 1 : x2 > x1 ? x2 - x1 + 1 : max_x - x1 + x2+1;
        public int size
                => width * height;

        public int x1;
        public int y1;
        public int x2;
        public int y2;

        public section(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        section clone => new section(x1, y1, x2, y2);
        public static int tile(int x, int max)
        {
            if (x >= max) x=x % max;
            while (x < 0) x += max;
            return x;
        }
        public void move_left()
        {
            x1 = tile(x1 - 1,max_x);
        }
        public void move_right()
        {
            x2 = tile(x2 + 1, max_x);
        }
        public void move_top()
        {
            y1 = tile(y1 - 1, max_y);
        }
        public void move_bottom()
        {
            y2 = tile(y2 + 1, max_y);
        }
        public bool can_move_left()
            => isvalid(tile(x1 - 1, max_x ), y1, x2, y2);
        public bool can_move_right()
             => isvalid(x1, y1, tile(x2 + 1, max_x ), y2);
        public bool can_move_top()
             => isvalid(x1, tile(y1 - 1, max_y ), x2, y2);
        public bool can_move_bottom()
            => isvalid(x1, y1, x2, tile(y2 + 1, max_y ));

        public static bool contains(List<section> l,section s)
        {
            foreach (var i in l)
            {
                if (i == s) return true;
            }
            return false;
        }
        public static List<section> expand(List<section> c)
        {

            var r = new List<section>();
            foreach (var i in c)
                r.Add(i);
            bool changed = false; 
            foreach (var j in c)
            {
                if (j.can_move_left())
                {
                    var s = j.clone;
                    s.move_left();
                    if(!contains(r,s))
                    {
                        r.Add(s);
                        changed = true;
                    }
                    
                }
                if (j.can_move_right())
                {
                    var s = j.clone;
                    s.move_right();
                    if (!contains(r, s))
                    {
                        r.Add(s);
                        changed = true;
                    }
                }
                if (j.can_move_top())
                {
                    var s = j.clone;
                    s.move_top();
                    if (!contains(r, s))
                    {
                        r.Add(s);
                        changed = true;
                    }

                }
                if (j.can_move_bottom())
                {
                    var s = j.clone;
                    s.move_bottom();
                    if (!contains(r, s))
                    {
                        r.Add(s);
                        changed = true;
                    }

                }
                j.can_expand = false;
            }
            if (changed) r=expand(r);
            return r;
        }
        bool can_expand = true;
        bool isvalid(int x1,int y1,int x2,int y2)
        {
            for (int i = x1; ;  i = tile(i + 1, max_x))
            {
                for (int j = y1; ; j = tile(j + 1, max_y))
                {
                    if (map[i, j] != 1)
                    {
                        return false;
                    }
                    if (j == y2) break;
                }
                if (i == x2) break;
            }
            return true;
        }
        public string result()
        {
            var result = new StringBuilder();
            
            List<string> curs = new List<string>();
            bool[] negatedx = new bool[xv.Length];
            bool[] cx= new bool[xv.Length];
            for (int j = 0; j < cx.Length; j++)
            {
                cx[j] = true;
            }
            bool[] negatedy = new bool[yv.Length];
            bool[] cy = new bool[yv.Length];
            for (int j = 0; j < cy.Length; j++)
            {
                cy[j] = true;
            }
            for (int i = x1;; i = tile(i + 1, max_x))
            {
                curs.Add(x[i]);
                if (i == x2) break;
            }
            for (int i = 0; i < curs.Count; i++)
            {
                for (int j = i; j < curs.Count; j++)
                {
                    for (int k = 0; k < curs[j].Length; k++)
                    {
                        if (curs[i][k] != curs[j][k]) cx[k] = false;
                        else if (curs[i][k] == '0') negatedx[k] = true;
                    }
                }
            }
            for (int i = 0; i < cx.Length; i++)
            {
                if (cx[i])
                {
                    result.Append(xv[i]);
                    if (negatedx[i]) result.Append('\'');
                }
            }
            curs = new List<string>();
            for (int i = y1 ;; i = tile(i + 1, max_y))
            {
                curs.Add(y[i]);
                if (i == y2) break;
            }
            for (int i = 0; i < curs.Count; i++)
            {
                for (int j = i; j < curs.Count; j++)
                {
                    for (int k = 0; k < curs[j].Length; k++)
                    {
                        if (curs[i][k] != curs[j][k]) cy[k] = false;
                        else if (curs[i][k] == '0') negatedy[k] = true;
                    }
                }
            }
            for (int i = 0; i < cy.Length; i++)
            {
                if (cy[i])
                {
                    result.Append(yv[i]);
                    if (negatedy[i])
                        result.Append('\'');
                }
            }
            return result.ToString();
        }
        public bool isneeded()
        {
            for (int i = x1; ; i = tile(i + 1, max_x))
            {
                for (int j = y1; ; j = tile(j + 1, max_y))
                {
                    if (needed[i, j] == true) return true;
                    if (j == y2) break;
                }
                if (i == x2) break;
            }
            return false;
        }
        public void set_used()
        {
            for (int i = x1;; i = tile(i + 1, max_x))
            {
                for (int j = y1; ; j = tile(j + 1, max_y))
                {
                    needed[i, j] = false;
                    if (j == y2) break;
                }
                if (i == x2) break;
            }
        }
        public static bool operator ==(section a, section b)
                => a.x1 == b.x1 && a.x2 == b.x2 && a.y1 == b.y1 && a.y2 == b.y2;
        public static bool operator !=(section a, section b)
                => a.x1 != b.x1 && a.x2 != b.x2 && a.y1 != b.y1 && a.y2 != b.y2;
    }
    
    
}
