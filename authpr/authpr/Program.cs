using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Security;
using System.Xml;
using System.Xml.Linq;
using System.Numeric;
//using Newtonsoft;

//using System.Threading;
//using System.Data.OleDb;
//using Microsoft.Office;
//using Microsoft.CSharp;
//using SharpEntropy;
//using Excel = Microsoft.Office.Interop.Excel;
//using System.Data;

namespace authpr
{
     
       

    class Program
    {
        
        

        //static int choice = 0;
        //static string path = "";
        static double favChar = 0;
        static double favWrd = 0;
        static double favEmo = 0;
        static double EmoNeu = 0;
        static double favAtt = 0;
        static double AttNeu = 0;
        static double O5Diff = 0;
        static double O5 = 0;
        static double NOA = 0;
        static double SpRDiff = 0;
        static double SpR = 0;
        static double FavSPOS = 0;
        static double FavEPOS = 0;
        static double FavPPOS = 0;
        static double FavPOS = 0;
        static double Diffr = 0;

        static string truthfile = "";

        static string[] SentencWindow = new string[3];
        //static string ConnectingSen = "";
        static List<string> ss1;
        static List<int> SenLen;
        static List<string> combinedindices;
        static string[] WinResult = new string[3];
        static List<int> Json = new List<int>();
        static Hashtable ht             = new Hashtable();
        static List<string> anger       = new List<string>();
        static List<string> relation    = new List<string>();
        static List<string> religion   = new List<string>();
        //static List<string> happy       = new List<string>();
        static List<string> love        = new List<string>();
        //static List<string> relaxed     = new List<string>();
        //static List<string> satisfied   = new List<string>();
        static List<string> urgency     = new List<string>();
        static List<string> negative    = new List<string>();
        static List<string> positive    = new List<string>();
        static List<string> politics    = new List<string>();
        static List<string> common      = new List<string>();
        static List<string> nature      = new List<string>();
        static List<string> tech        = new List<string>();




        static string male   = "";
        static string female = "";
        static int listscore = 0;
        static string resultstring = "";
        //static HASH_POS.POS hp = new HASH_POS.POS();
        static Hashtable FavWords = new Hashtable();
        static Hashtable FavPairWords = new Hashtable();
        static Hashtable DocTrend = new Hashtable();
        static int currentindex= 0;
        
        


        static void GetFavWord()
        {
            Dictionary<string, int> chardic = new Dictionary<string, int>();
            string[] str = SentencWindow[currentindex].Split(' ');
            FavWords.Clear();
           
            foreach (string aa in str)
            {
                if (aa == " " || aa == "" || aa == ".." || aa == "..." || aa == "-")
                    continue;
                aa.Trim('.','?',';', ',', '"', '!', '(', ')', '[',']','_','%','@','-',':',' ');
                if (chardic.ContainsKey(aa.ToLower()))
                    chardic[aa.ToLower()] += 1;
                else
                    chardic[aa.ToLower()] = 1;
            }
            try
            {
                var sortedDict = from entry in chardic orderby entry.Value descending select entry;
                string st = "";
                int i = 0;
                foreach( KeyValuePair<string,int> kv in sortedDict)
                {
                    double score = (double) kv.Value / str.Count();
                    //if(i == 0)
                    //st +=  kv.Key +"|"+ score.ToString();
                    //else
                    //    st += "," + kv.Key + "|" + score.ToString();
                    FavWords.Add(kv.Key, score);
                    if (i == 100)
                        break;
                    i++;
                }
               // resultstring = st;
            }
            catch (Exception ex)
            { resultstring = " error "; }
            //WinResult.Add("{"+ sortedDict.First().Key.ToString());
            
            GetFavWordPair(str);
            GetOrdinaryWordScore(chardic);
            GetSentenceExpression(chardic);
            GetSentenceAttitude(chardic);

        }

        static void GetFavWordPair(string[] str)
        {
            Dictionary<string, int> chardic = new Dictionary<string, int>();
            //string[] str = SentencWindow[currentindex].Split(' ');
            int cnt = str.Count() -1;
            FavPairWords.Clear();
          
            for (int i = 0; i < str.Count() - 1; i++)
            {
                string aa = str[i].ToLower() + " " + str[i + 1].ToLower();
                if (chardic.ContainsKey(aa))
                    chardic[aa] += 1;
                else
                    chardic[aa] = 1;
            }
            
            try
            {
                var sortedDict = from entry in chardic orderby entry.Value descending select entry;
                //WinResult.Add("{ " + sortedDict.First().Key.ToString());
                string st = "";
                int i = 0;
                foreach (KeyValuePair<string, int> kv in sortedDict)
                {
                    double score = (double)kv.Value / str.Count();
                    //if (i == 0)
                    //    st += kv.Key + "|" + score.ToString();
                    //else
                    //    st += "," + kv.Key + "|" + score.ToString();
                    FavPairWords.Add(kv.Key, score);
                    if (i == 100)
                        break;
                    i++;
                }
                //resultstring += " { " + st;
            }
            catch (Exception ex)
            { resultstring += " { error"; }
        }
        static void GetOrdinaryWordScore(Dictionary<string, int> dict)
        {
            int score = 0;
            int total = 0;
            DocTrend.Clear();
            /*foreach (KeyValuePair<string, int> aa in dict)
            {
                if (common.Contains(aa.Key))
                {

                    score += aa.Value;
                }
                total += aa.Value;
            }*/
            double dd = (double) score /  total;
            //WinResult.Add("{" + dd.ToString());
            //resultstring += " { common|" + dd.ToString();
            DocTrend.Add("common",dd);
    
        }

        static void GetSentenceExpression(Dictionary<string, int> chardic)
        {
            Dictionary<string, int> exResult = new Dictionary<string,int>();
            exResult.Add("anger",0); exResult.Add("love",0); exResult.Add("religion",0); exResult.Add("relation",0); ;
            exResult.Add("politics", 0); exResult.Add("tech", 0);/* exResult.Add("urgency", 0);*/ exResult.Add("nature", 0);
            uint total = 0; 
            foreach (KeyValuePair<string, int> aa in chardic)
            {
               if (anger.Contains(aa.Key))
                    exResult["anger"] += aa.Value;
                if (love.Contains(aa.Key))
                    exResult["love"] += aa.Value;
                if (religion.Contains(aa.Key))
                    exResult["religion"] += aa.Value;
                if (nature.Contains(aa.Key))
                    exResult["nature"] += aa.Value;
                /*if (urgency.Contains(aa.Key))
                    exResult["urgency"] += aa.Value;*/
                if (tech.Contains(aa.Key))
                    exResult["tech"] += aa.Value;
                if (politics.Contains(aa.Key))
                    exResult["politics"] += aa.Value;
                if (relation.Contains(aa.Key))
                    exResult["relation"] += aa.Value;


                total += (uint)aa.Value;

            }

            int i = 0;
            string st = "";
            foreach (KeyValuePair<string, int> kv in exResult)
            {
                double score = (double)kv.Value / total;
                //if (i == 0)
                //    st += kv.Key + "|" + score.ToString();
                //else
                //    st += "," + kv.Key + "|" + score.ToString();
                //i++;
                DocTrend.Add(kv.Key, score);
            }
            //resultstring += " { " + st;
            //resultstring += " { " + exResult["anger"].ToString() + " " + exResult["confusion"].ToString() + " " + exResult["curiosity"].ToString() + " " + exResult["happy"].ToString() + " " + exResult["inspired"] +" "+ exResult["relaxed"];

            
        }

        static void GetSentenceAttitude(Dictionary<string, int> chardic)
        {
            int neg = 0;
            int pos = 0;
            int total = 0;
          /*   foreach (KeyValuePair<string, int> aa in chardic)
             {
                 if (negative.Contains(aa.Key))
                     neg += aa.Value;
                 else if (positive.Contains(aa.Key))
                     pos += aa.Value;

                 total += aa.Value;
             }
            */
            double dpos = (double)pos / total;
            double dneg = (double)neg / total;
            //resultstring += " { " + "pos|" + dpos.ToString() + ", neg|" + dneg.ToString();

            DocTrend.Add("pos", dpos);
            DocTrend.Add("neg",dneg);
            
        }

        static void CreateOutput(string outputPath,string[] id,string lang, string result)
        {
            try
            {
                string[] reg_gen = result.Split(',');
                XmlWriter w;
                w = XmlWriter.Create(outputPath);
                w.WriteStartDocument();
                w.WriteStartElement("author");
                w.WriteAttributeString("id", id[0]);
                w.WriteAttributeString("lang", lang);
                w.WriteAttributeString("variety", reg_gen[0].Trim());
                w.WriteAttributeString("gender", reg_gen[1].Trim());
                w.WriteEndElement();
                w.WriteEndDocument();
                w.Close();
                Console.WriteLine("->" + id[0] + "[" + result + "]");
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString()); }
            
        }

        static void LoadFiles(string filepath)
        {
                XmlDocument doc = new XmlDocument();
                string finalData = "";
                try
                {
                        doc.Load(filepath /*+ "/" + ww[0] + ".xml"*/);
                        string cDataNode = doc.SelectSingleNode("author/documents").InnerText;
                        finalData = Regex.Replace(cDataNode, "<.,*? \">", " ");
                        //finalData = Regex.Replace(finalData, @"[^\w\.@-]", string.Empty, RegexOptions.Compiled);
                        finalData = Regex.Replace(finalData, @"http[^\s]+", " ");
                        finalData = Regex.Replace(finalData, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", " ");
                        finalData = Regex.Replace(finalData, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", " ");
                        
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
                SentencWindow[0] = finalData;
                currentindex = 0;
                GetFavWord();
        }
        

        static void Listfiller(List<string> lst, string filepath)
        {
            lst.Clear();
            FileStream fss = new FileStream(filepath, FileMode.Open);
            StreamReader br = new StreamReader(fss,Encoding.UTF8);
            while (true)
            {
                try
                {
                    string a = br.ReadLine().ToLower();//WF.ReadLine().Split('\t');
                    if (a != "" && a != " " && !lst.Contains(a))
                    {
                        lst.Add(a);
                    }
                }
                catch (Exception ex)
                { br.Close(); break; }
            }

            fss.Close();
        }


        static void LstFillerCall(string fpath)
        {
            Listfiller(anger, fpath + "anger.txt");
            Listfiller(common, fpath + "common.txt");
            Listfiller(religion, fpath + "religion.txt");
            Listfiller(love, fpath + "love.txt");
            Listfiller(nature, fpath + "nature.txt");
            Listfiller(negative, fpath + "negative.txt");
            Listfiller(positive, fpath + "positive.txt");
            Listfiller(tech, fpath + "tech.txt");
            Listfiller(politics, fpath + "politics.txt");
            Listfiller(relation, fpath + "relation.txt");
        }


        //------------------------------------                                                                                              -----------------------------------------------------//
        //================================================================================================    MAIN    ======================================//
        static void Main(string[] args)
        {
            string InPath = "./pan/";
            string OutPath = "./Results/";

            

            if (args.Count() == 0)
            {
                Console.WriteLine("NO PATH TO INPUT AND OUTPUT DIRECTORIES PROVIDED (e.g. c:/intrinsic/authpr -i c:/pan/intrinsic -o c:/output/) or (e.g. c:/intrinsic/authpr c:/pan/intrinsic c:/output/)");
                //Environment.Exit(0);
            }
            else
            {
                //string[] ars = args.Split(' ');
                if (args[0] == "-i" && args[2] == "-o")
                {
                    InPath = args[1];
                    OutPath = args[3];
                }
                else
                {
                    InPath = args[0];
                    OutPath = args[1];
                }

                //Console.WriteLine(args[1] + "------------" + args[3]);
            }
           
            ss1 = new List<string>();
            
            //string fo = OutPath;
            DirectoryInfo di;
            DirectoryInfo[] sdi = null;
            try
            {
               di = new DirectoryInfo(InPath); 
               sdi = di.GetDirectories();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString() + ":" + "Please Provide correct input path"); }
             
            string wordsdir = AppDomain.CurrentDomain.BaseDirectory +"/";
            string fpath = "";
            foreach (var dir in sdi)
            {

                if (dir.Name == "ar")
                {
                    //continue;
                    fpath = wordsdir + "arabic/";
                    LstFillerCall(fpath);
                    System.IO.Directory.CreateDirectory(OutPath + "/ar/");
                    Class1 c1 = new Class1("ar");
                    foreach (FileInfo fi in dir.GetFiles("*.xml"))
                    {
                            try
                            {
                                LoadFiles(fi.FullName);
                                string region_gender = c1.GetDifference(FavWords, FavPairWords, DocTrend);
                                CreateOutput(OutPath + "/ar/" + fi.Name, fi.Name.Split('.'), "ar", region_gender);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("error:" + ex.ToString());
                            }

                    }
                     
                }
                else if (dir.Name == "en")
                {
                    //continue;
                    fpath = wordsdir + "english/";
                    LstFillerCall(fpath);
                    System.IO.Directory.CreateDirectory(OutPath + "/en/");
                    Class1 c1 = new Class1("en");
                    foreach (FileInfo fi in dir.GetFiles("*.xml"))
                    {
                            try
                            {
                                
                                LoadFiles(fi.FullName);
                                string region_gender = c1.GetDifference(FavWords, FavPairWords, DocTrend);
                                CreateOutput(OutPath + "/en/" + fi.Name, fi.Name.Split('.'), "en", region_gender);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("error:" + ex.ToString());
                            }
                        
                    }
                }
                else if (dir.Name == "es")
                {
                    //continue;
                    fpath = wordsdir + "spanish/";
                    LstFillerCall(fpath);
                    System.IO.Directory.CreateDirectory(OutPath + "/es/");
                    Class1 c1 = new Class1("es");
                    foreach (FileInfo fi in dir.GetFiles("*.xml"))
                    {
                            try
                            {
                                
                                LoadFiles(fi.FullName);
                                string region_gender = c1.GetDifference(FavWords, FavPairWords, DocTrend);
                                CreateOutput(OutPath + "/es/" + fi.Name, fi.Name.Split('.'), "es", region_gender);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("error:" + ex.ToString());
                            }
                       }
                       
                }
                else if (dir.Name == "pt")
                {
                    fpath = wordsdir + "portugese/";
                    LstFillerCall(fpath);
                    System.IO.Directory.CreateDirectory(OutPath + "/pt/");
                    Class1 c1 = new Class1("pt");
                    foreach (FileInfo fi in dir.GetFiles("*.xml"))
                    //double precision = 0;
                    //double aprec = 0;
                    //foreach (FileInfo fi in dir.GetFiles("*.txt"))
                    {
                        //StreamReader sr = new StreamReader(fi.FullName);
                        //int total = 0;
                        //while (!sr.EndOfStream)
                        //{
                            //string asa = sr.ReadLine();
                            //string[] ww = asa.Split(':');
                            //total++;
                            try
                            {
                                //LoadFiles("./pan/pt/" + ww[0] + ".xml");
                                LoadFiles(fi.FullName);
                                string region_gender = c1.GetDifference(FavWords, FavPairWords, DocTrend);
                                //string[] dec = region_gender.Split(',');
                                //if (dec[1] == ww[3])
                                //    precision += 0.5;
                                //if ((dec[0] == ww[6]))
                                //    precision += 0.5;
                                //if (dec[1] == ww[3] && dec[0] == ww[6])
                                //    aprec += 1;
                                CreateOutput(OutPath + "/pt/" + fi.Name, fi.Name.Split('.'), "pt", region_gender);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("error:" + ex.ToString());
                            }
                       // }
                        //double res = (double)precision / total;
                        //double ares = (double)aprec / total;
                        //System.IO.File.AppendAllText("./AAA.txt", "|" + res.ToString() + "," + ares.ToString());
                    }
                }
                
                
                

                
            }
        } 
    }
}
