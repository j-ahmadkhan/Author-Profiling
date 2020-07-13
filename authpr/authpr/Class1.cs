using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;

namespace authpr
{

    class Class1
    {
        string wordsdir = AppDomain.CurrentDomain.BaseDirectory + "Configs/";
        Dictionary<string, double> region; //= new Dictionary<string, double>();
        public Hashtable male; //= new Dictionary<string, double>();
        public Hashtable female;// = new Dictionary<string, double>();

        Dictionary<string, double> ar; // = new Dictionary<string,double>() ; //{"gulf","levantine","maghrebi","egypt"};
        Dictionary<string, double> en; //= new Dictionary<string,double>() ;// = { "australia", "canada", "great britain", "ireland", "new zealand", "united states" };
        Dictionary<string, double> es;// = new Dictionary<string,double>() ;//= { "argentina", "chile", "colombia", "mexico", "peru", "spain", "venezuela" };
        Dictionary<string, double> pt; //= new Dictionary<string, double>();// = { "portugal", "brazil" };

        string reg_dir;
        Hashtable Curfile;
        //for final comparison just in case we get the wrong region, we have m and f genders of overall language to over rule the regional found gender
        public double reg_male;
        public double reg_female;
        public double lang_male;
        public double lang_female;
        
        public Hashtable[] FavWords;
        public Hashtable[] FavPairWords;
        public Dictionary<string,double>[] DocTrend;
        public Class1(string dir)
        {
           reg_dir = dir;
           reg_male = 0;
           reg_female = 0;
           lang_male = 0;
           lang_female = 0;

            if (reg_dir == "ar")
            {
                Dictionary<string, double> ar = new Dictionary<string, double>();
                ar.Add("gulf", 0); ar.Add("levantine", 0); ar.Add("maghrebi", 0); ar.Add("egypt", 0);
                region = ar;
                FavWords = new Hashtable[4 + 2 /* for male and female geder */];
                FavPairWords = new Hashtable[4 + 2 /* for male and female geder */];
                DocTrend = new Dictionary<string,double>[4 + 2 /* for male and female geder */];
                for (int i = 0; i < 6; i++)
                {
                    FavWords[i] = new Hashtable();
                    FavPairWords[i] = new Hashtable();
                    DocTrend[i] = new Dictionary<string,double>();
                }
               

                wordsdir = wordsdir + "ar";
            }
            else if (reg_dir == "en")
            {
                Dictionary<string, double> en = new Dictionary<string, double>();
                en.Add("australia", 0); en.Add("canada", 0); en.Add("great britain", 0); en.Add("ireland", 0); en.Add("new zealand", 0); en.Add("united states", 0);
                region = en;
                FavWords = new Hashtable[6 + 2 /* for male and female geder */];
                FavPairWords = new Hashtable[6 + 2 /* for male and female geder */];
                DocTrend = new Dictionary<string, double>[6 + 2 /* for male and female geder */];
                for (int i = 0; i < 8; i++)
                {
                    FavWords[i] = new Hashtable();
                    FavPairWords[i] = new Hashtable();
                    DocTrend[i] = new Dictionary<string, double>();
                }
                wordsdir = wordsdir + "en";
            }
            else if (reg_dir == "es")
            {
                Dictionary<string, double> es = new Dictionary<string, double>();
                es.Add("argentina", 0); es.Add("chile", 0); es.Add("colombia", 0); es.Add("mexico", 0); es.Add("peru", 0); es.Add("spain", 0); es.Add("venezuela", 0);
                region = es;
                FavWords = new Hashtable[7 + 2 /* for male and female geder */];
                FavPairWords = new Hashtable[7 + 2 /* for male and female geder */];
                DocTrend = new Dictionary<string, double>[7 + 2 /* for male and female geder */];
                for (int i = 0; i < 9; i++)
                {
                    FavWords[i] = new Hashtable();
                    FavPairWords[i] = new Hashtable();
                    DocTrend[i] = new Dictionary<string, double>();
                }
                wordsdir = wordsdir + "es";
            }
            else if (reg_dir == "pt")
            {
                Dictionary<string, double> pt = new Dictionary<string, double>();
                pt.Add("portugal", 0); pt.Add("brazil", 0);
                region = pt;
                FavWords = new Hashtable[2 + 2 /* for male and female geder */];
                FavPairWords = new Hashtable[2 + 2 /* for male and female geder */];
                DocTrend = new Dictionary<string, double>[2 + 2 /* for male and female geder */];
                for (int i = 0; i < 4; i++)
                {
                    FavWords[i] = new Hashtable();
                    FavPairWords[i] = new Hashtable();
                    DocTrend[i] = new Dictionary<string, double>();
                }
                wordsdir = wordsdir + "pt";
            }

            LoadRegions(false,null);
            
        }

        public string GetDifference(Hashtable FileFavWrds, Hashtable FileWrdPairs, Hashtable FileDocTrend)
        {
            string[] KeyCollection = region.Keys.ToArray<string>();
            int CurrentRegion = 0;
            foreach(string kv in KeyCollection)
            {
                region[kv] = 0; // set 0 for each new document
                //foreach (KeyValuePair<object, object> fkv in FileFavWrds)
                foreach (var fkv in FileFavWrds.Keys)
                {
                     if (FavWords[CurrentRegion].ContainsKey(fkv))
                         region[kv] += (double)FileFavWrds[fkv] + (double)FavWords[CurrentRegion][(string)fkv];
                }
                foreach (var fkv in FileWrdPairs.Keys)
                {
                    if (FavPairWords[CurrentRegion].ContainsKey(fkv))
                        region[kv] += (double)FileWrdPairs[fkv] + (double)FavPairWords[CurrentRegion][(string)fkv];
                       

                }

                CurrentRegion ++;
            }
        
            
            //Document Trend Analysis
            double[] TrendScore = new double[region.Count()];
            double[] fileTrendScore = new double[region.Count()]; 
            for (int i = 0; i < region.Count(); i++)
            {
                foreach (KeyValuePair<string, double> kkv in DocTrend[i])
                { TrendScore[i] += kkv.Value; }
                foreach(var kkv in FileDocTrend.Keys)
                { fileTrendScore[i] += (double)FileDocTrend[kkv]; }

                TrendScore[i] = Math.Abs(TrendScore[i] - fileTrendScore[i]);
            }

            CurrentRegion = 0;
            foreach(string kv in region.Keys)
            {
                if (TrendScore[CurrentRegion] == TrendScore.Min()) // Regional Trend score with minimum value difference is closest to the current file trend 
                { region[kv] += ((TrendScore[CurrentRegion] * 3.5) ); break; } // 4 to give advantage to trend winner
                CurrentRegion++;
            }

            string filereg = region.OrderByDescending(x => x.Value).First().Key; // Get The highest region score

            //Now we have found the region of file, Lets find the gender with same procedure--- but first load gender of selected region
            LoadRegions(true, filereg + "-"); // Load region based gender

            GetGender(FileFavWrds, FileWrdPairs, FileDocTrend, "regional");

            LoadRegions(true, null); // Load language based gender

            return filereg + "," + GetGender(FileFavWrds, FileWrdPairs, FileDocTrend,"language");  

        }

        public string GetGender(Hashtable FileFavWrds, Hashtable FileWrdPairs, Hashtable FileDocTrend, string type_of_gender)
        {
            double[] gender = { 0, 0 }; // set 0 for each gender male and female
            int gen_counter = 0;
            for (int CurrentRegion = region.Count(); CurrentRegion < region.Count() + 2; CurrentRegion++ )
            {
                
                foreach (var fkv in FileFavWrds.Keys)
                {
                    if (FavWords[CurrentRegion].ContainsKey(fkv))
                        gender[gen_counter] += (double)FileFavWrds[fkv] + (double)FavWords[CurrentRegion][(string)fkv];
                }
                foreach (var fkv in FileWrdPairs.Keys)
                {
                    if (FavPairWords[CurrentRegion].ContainsKey(fkv))
                        gender[gen_counter] += (double)FileWrdPairs[fkv] + (double)FavPairWords[CurrentRegion][(string)fkv];
                }

                gen_counter++;
            }

            //Document Trend Analysis
            double[] TrendScore = {0,0};
            double[] fileTrendScore = { 0, 0 };
            for (int i = 0; i < 2; i++)
            {
                foreach (KeyValuePair<string, double> kkv in DocTrend[region.Count() + i])
                { TrendScore[i] += kkv.Value; }
                foreach (var kkv in FileDocTrend.Keys)
                { fileTrendScore[i] += (double)FileDocTrend[kkv]; }

                TrendScore[i] = Math.Abs(TrendScore[i] - fileTrendScore[i]);
            }

            if (TrendScore[0] < TrendScore[1]) // Regional Trend score with minimum value difference is closest to the current file trend 
            { gender[0] += TrendScore[0] * 3.5; /*3.5 is the added multiplicity threshold to give advantage to trend winner gender*/; }
            else if (TrendScore[1] < TrendScore[0]) // Regional Trend score with minimum value difference is closest to the current file trend 
            { gender[1] += TrendScore[1] * 3.5; /*3.5 is the added multiplicity threshold to give advantage to trend winner gender*/  }

            //if (gender[1] > gender[0])
            //    return "female";
            //else
            //    return "male";

            if (type_of_gender == "regional")
            {
                reg_male = gender[0];
                reg_female = gender[1];
                return "Deciding gender....on the basis of language too";
            }
            else if (type_of_gender == "language")
            {
                lang_male = gender[0];
                lang_female = gender[1];
                if ((reg_male > reg_female) && (lang_male > lang_female))
                    return "male";
                if ((reg_male > reg_female) && (lang_male < lang_female)) // in case of difference of opinion in regional and language genders the greater value will prevail
                {
                    if (lang_female > reg_male)
                        return "female";
                    else
                        return "male";
                }
                if ((reg_male < reg_female) && (lang_male < lang_female))
                    return "female";
                if ((reg_male < reg_female) && (lang_male > lang_female)) // in case of difference of opinion in regional and language genders the greater value will prevail
                {
                    if (lang_male > reg_female)
                        return "male";
                    else
                        return "female";
                }

                return "male"; // default :->
            }
            return "male"; // default :->
        }

        public void LoadRegions(bool gender, string gender_region)
        {
            int num = 0;
            int gender_counter = 0;

            FileStream sr;
            if (gender)
            {
                //MAKE SURE TO CLEAR GENDER HASH TABLES FOR EACH FILE TO COME
                num = region.Count();
                FavWords[num].Clear(); // MALE
                FavWords[num + 1].Clear(); // FEMALE
                FavPairWords[num].Clear(); // MALE
                FavPairWords[num + 1].Clear(); // FEMALE
                DocTrend[num].Clear(); // MALE
                DocTrend[num + 1].Clear(); // FEMALE
            }

            foreach (KeyValuePair<string, double> kv in region)
            {
                if (gender && (gender_counter == 0) /*gender counter for getting gender based files*/)
                {
                    sr = new FileStream(wordsdir + "/gender/" + gender_region + "male.dat", FileMode.Open);
                    gender_counter++;
                }
                else if (gender && (gender_counter == 1))
                {
                    sr = new FileStream(wordsdir + "/gender/" + gender_region + "female.dat", FileMode.Open);
                    gender_counter++;
                }
                else
                sr = new FileStream(wordsdir + "/region/" + kv.Key + ".dat", FileMode.Open);
                BinaryReader br = new BinaryReader(sr, Encoding.UTF8);
                //while (true)
                {
                    try
                    {
                        string a = br.ReadString();//WF.ReadLine().Split('\t');
                        br.Close();
                        sr.Close();
                     
                        string[] b          = a.Split('{');
                        string[] wrds       = b[0].Split(',');
                        string[] wrdspair   = b[1].Split(',');
                        string[] exp        = b[3].Split(',');
                        string[] att        = b[4].Split(',');
                        

                        foreach (string s in wrds)
                        {
                            string[] splt = s.Split('|');
                            try
                            {
                                FavWords[num].Add(splt[0], Double.Parse(splt[1]));
                            }
                            catch(Exception ex) { continue; }
                        }
                        foreach (string s in wrdspair)
                        {
                            string[] splt = s.Split('|');
                            try
                            {
                                FavPairWords[num].Add(splt[0], Double.Parse(splt[1]));
                            }
                            catch { continue; }
                        }
                        string[] comm = b[2].Split('|');
               
                        DocTrend[num].Add(comm[0], Double.Parse(comm[1]));    
                       
                        foreach (string s in exp)
                        {
                            string[] splt = s.Split('|');
                            try
                            {
                                DocTrend[num].Add(splt[0], Double.Parse(splt[1]));
                            }
                            catch { continue; }
                        }
                        foreach (string s in att)
                        {
                            string[] splt = s.Split('|');
                            try
                            {
                                DocTrend[num].Add(splt[0], Double.Parse(splt[1]));
                            }
                            catch { continue; }
                        }

                    }
                    catch (EndOfStreamException ex)
                    { continue; }
                    num++;
                    if (gender_counter == 2)
                        break;
                }
            }

        }



    }
   
}
