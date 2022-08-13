using AutomationTest.reporting.serilog;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest.commonutils
{
    public class Util
    {
        private static readonly ILogger logger = LoggerConfig.Logger;

        public static Random random = new Random();

        public enum Mode
        {
            ALPHA, ALPHANUMERIC, NUMERIC
        }

        public static String getRootPath()
        {
            return Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        }

        public static String getSolutionPath()
        {
            return Directory.GetParent(getRootPath()).FullName;
        }


        public static String getCurrentDate()
        {

            try
            {
                DateTime today = DateTime.Now;

                String date = today.ToString("MM-dd-yyyy");
                date = date.Replace(":", "_");
                date = date.Replace(" ", "_");
                date = date.Replace(".", "_");
                date = date.Replace("-", "_");
                return date;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                logger.Here().Error(e.Message);
                logger.Here().Error(e.StackTrace);
                logger.Here().Error(e.InnerException.ToString());
                throw;
            }
        }

        /// <summary>
        /// This method is get the Current Time in the format 'hh-mm-ss'
        /// </summary>
        /// <returns>Current Time in String Format</returns>
        public static String getCurrentTime()
        {
            try
            {
                String result = DateTime.Now.TimeOfDay.ToString();

                result = result.Replace(":", "_");
                result = result.Replace(" ", "_");
                result = result.Replace(".", "_");
                return result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //logger.Error(e.Message);
                //logger.Error(e.StackTrace);
                //logger.Error(e.InnerException);
                throw e;
            }

        }

        public void ExecuteDMLQueries(String ConnectionString, String Query)
        {

            string WriteCommandText = Query;

            try
            {

                SqlConnection thisConnection = new SqlConnection(ConnectionString);
                thisConnection.Open();

                SqlCommand thisCommand = thisConnection.CreateCommand();


                thisCommand.CommandText = WriteCommandText;
                thisCommand.ExecuteNonQuery();
                thisConnection.Close();
            }
            catch (SqlException SE)
            {
                Console.WriteLine("Unable to write the values in the database. Check the Connection Strings");
                Console.WriteLine(SE.Message);
                Console.WriteLine(SE.StackTrace);
                Console.WriteLine(SE.InnerException);
                throw SE;
            }

            catch (Exception e)
            {
                Console.WriteLine("Unable to write the values in the database. Check the Connection Strings");

                Console.WriteLine(e.Message);
                logger.Here().Error(e.Message);
                logger.Here().Error(e.StackTrace);
                logger.Here().Error(e.InnerException.ToString());
                throw;
            }

        }



        public String generateRandomString(int length, Mode mode)
        {

            StringBuilder buffer = new StringBuilder();
            String characters = "";

            switch (mode)
            {

                case Mode.ALPHA:
                    characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;

                case Mode.ALPHANUMERIC:
                    characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                    break;

                case Mode.NUMERIC:
                    characters = "1234567890";
                    break;
            }
            int charactersLength = characters.Length;

            for (int i = 0; i < length; i++)
            {

                double index = random.Next(charactersLength);

                buffer.Append(characters[((int)index)]);
            }
            return buffer.ToString();
        }



        public String CreateDateFolder(String homePath)
        {
            try
            {
                DirectoryInfo d;

                d = new DirectoryInfo(homePath + "\\Results");
                if (!d.Exists)
                {
                    d.Create();
                }

                String date = "Run_" + Util.getCurrentDate();

                String resultsFolder = homePath + "\\Results\\" + date;

                d = new DirectoryInfo(resultsFolder);
                if (!d.Exists)
                {
                    d.Create();
                }

                return resultsFolder;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //logger.Error(e.Message);
                //logger.Error(e.StackTrace);
                //logger.Error(e.InnerException);
                throw e;
            }
        }

        public String CreateDirectory(String DirectoryPath, String DirectoryName)
        {
            try
            {
                String DirectoryFulPath = DirectoryPath + "\\" + DirectoryName;
                DirectoryInfo d1 = new DirectoryInfo(DirectoryFulPath);
                if (!d1.Exists)
                {
                    d1.Create();
                }

                return DirectoryFulPath;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //logger.Error(e.Message);
                //logger.Error(e.StackTrace);
                //logger.Error(e.InnerException);
                throw e;
            }
        }


        public string getRandom(String value)
        {
            if (value.ToLower().Contains("name"))
            {
                value = generateRandomString(random.Next(5, 20), Mode.ALPHA);
            }
            else if (value.ToLower().Contains("dob"))
            {
                value = getDateOfBirth(int.Parse(value));
            }

            else if (value.ToLower().Contains("ssn"))
            {
                value = GetRandomSSN();
            }
            else if (value.ToLower().Contains("ssn_dash"))
            {
                value = GetRandomSSN("-");
            }
            else if (value.ToLower().Contains("psuedossn"))
            {
                value = GetRandomSSN();
            }
            else if (value.ToLower().Contains("psuedossn_dash"))
            {
                value = GetRandomSSN("-");
            }
            else if (value.ToLower().Contains("individualid"))
            {
                value = generateRandomString(9, Mode.NUMERIC);
            }
            else if (value.ToLower().Contains("number"))
            {
                List<string> values = value.Split(';').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                int length = values.Count > 1 && values != null ? int.Parse(values[1]) : 10;
                value = generateRandomString(length, Mode.NUMERIC);
            }
            else if (value.ToLower().Contains("alphanum"))
            {
                value = generateRandomString(30, Mode.ALPHANUMERIC);
            }
            else if (value.ToLower().Contains("uuid") || value.ToLower().Contains("guid"))
            {
                Guid guid = Guid.NewGuid();
                value = guid.ToString();
            }

            return value;
        }
        /// <summary>
        /// Author: Krushna Dipayan Majhi
        /// Evaulates the Date of Birth based on the Age given
        /// </summary>
        /// <param name="Age">Age value (-1 = Evaluates Date of Birth with Random Age)</param>
        /// <param name="datTimeFormat">DateTimeFormat</param>
        /// <returns></returns>
        public string getDateOfBirth(int Age = -1, string datTimeFormat = "MM/dd/yyyy")
        {
            Random r = new Random();
            string Date;
            int Daterange;

            Age = Age == -1 ? r.Next(100) : Age;

            try
            {
                int NoOfdays = (Convert.ToInt32(Age)) * 365 + Convert.ToInt32(((Convert.ToInt32(Age)) / 4));
                Daterange = r.Next(NoOfdays + 1, NoOfdays + 364);
                Date = DateTime.Today.AddDays(-Daterange).ToString(datTimeFormat);
            }
            catch (Exception e)
            {
                //logger.Error("Unable to Add Date Of Birth due to error: " + e.ToString());
                throw;
            }
            return Date;
        }

        public string GetRandomSSN(string delimiter = "")
        {
            int iThree = GetRandomNumber(132, 899);
            int iTwo = GetRandomNumber(12, 83);
            int iFour = GetRandomNumber(1423, 9211);
            return iThree.ToString() + delimiter + iTwo.ToString() + delimiter + iFour.ToString();
        }

        public string GetRandomPsuedoSSN(string delimiter = "")
        {
            int iThree = GetRandomNumber(911, 988);
            int iTwo = GetRandomNumber(12, 83);
            int iFour = GetRandomNumber(1423, 9211);
            return iThree.ToString() + delimiter + iTwo.ToString() + delimiter + iFour.ToString();
        }

        public int GetRandomNumber(int min, int max)
        {
            Random getrandom = new Random();
            return getrandom.Next(min, max);
        }


        /// <summary>
        /// Reads entire Table(If Specific querry not Provided)
        /// </summary>
        /// <param name="thisConnection"></param>
        /// <param name="TableName">Name of the Table</param>
        /// <param name="InputQuerry"></param>
        /// <returns> A dictionary with Key as Column name and Values as data present in the respective colomn</returns>

        public Dictionary<string, List<string>> ReadDBTable(SqlConnection thisConnection, string InputQuerry)
        {
            try
            {
                //string InputQuerry = string.Format(IQuerry, TableName);
                SqlCommand thisCommand = new SqlCommand(InputQuerry, thisConnection);
                List<List<string>> TableMap = new List<List<string>>();
                SqlDataReader readdata = thisCommand.ExecuteReader();
                try
                {
                    Dictionary<string, List<string>> dbMap = new Dictionary<string, List<string>>();
                    if (readdata.HasRows)
                    {
                        while (readdata.Read())
                        {
                            for (int i = 0; i < readdata.FieldCount; i++)
                            {
                                if (dbMap.ContainsKey(readdata.GetName(i)))
                                {
                                    dbMap[readdata.GetName(i)].Add(readdata.GetValue(i).ToString());
                                }
                                else
                                {
                                    List<string> rowData = new List<string>();
                                    rowData.Add(readdata.GetValue(i).ToString());
                                    dbMap.Add(readdata.GetName(i), rowData);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Records Found in Database, This may be due to incorrect Querry provided");
                    }
                    readdata.Close();
                    return dbMap;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    readdata.Close();
                    throw e;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }


        public List<string> GetColumns(SqlDataReader readData)
        {
            List<string> colList = new List<string>();
            try
            {
                for (int i = 0; i < readData.FieldCount; i++)
                {
                    colList.Add(readData.GetName(i));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return colList;
        }


        public String CreateResultsFolder(String homePath, String ResultsFolderName = "\\TestResults")
        {
            try
            {
                DirectoryInfo d;

                d = new DirectoryInfo(homePath + ResultsFolderName);
                if (!d.Exists)
                {
                    d.Create();
                }

                String date = getCurrentDate();

                String ResultsFolder_Date = homePath + ResultsFolderName + "\\Day_ " + date;

                d = new DirectoryInfo(ResultsFolder_Date);
                if (!d.Exists)
                {
                    d.Create();
                }
                String Time = "Run_" + getCurrentTime();

                String ResultsFolder_Time = ResultsFolder_Date + "\\" + Time;

                d = new DirectoryInfo(ResultsFolder_Time);
                if (!d.Exists)
                {
                    d.Create();
                }


                return ResultsFolder_Time;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //logger.Error(e.Message);
                //logger.Error(e.StackTrace);
                //logger.Error(e.InnerException);
                throw e;
            }
        }

        public string ConvertFromBase64ToText(string Base64Text)
        {
            byte[] data = Convert.FromBase64String(Base64Text);
            string ConvertedText = Encoding.UTF8.GetString(data);
            return ConvertedText;
        }

        public string ReplacePlaceholders(string FilePath, Dictionary<String, String> placeHolders, String prefix = "#*", String suffix = "*#")
        {
            string XMLData = File.ReadAllText(FilePath);
            return placeHolders.Aggregate(XMLData, (current, value) => current.Replace(prefix + value.Key + suffix, value.Value));
        }

        public static List<Dictionary<String, String>> getTestData(string WorkbookPath, string sheetName, List<String> whereClause = null)
        {
            ExcelUtilities exUtil = new ExcelUtilities();

            Dictionary<String, List<String>> excelData = new Dictionary<String, List<String>>();
            if (whereClause == null) 
            {
                excelData = exUtil.ReadWorkSheet(WorkbookPath, sheetName);
            }
            else
            {
                excelData = exUtil.fetchWithCondition(WorkbookPath, sheetName, whereClause);
            }

            List<Dictionary<String, String>> testDatas = new List<Dictionary<string, string>>();
            HashSet<String> headers = excelData.Keys.ToHashSet();
            for (int i = 0; i < excelData.Values.ElementAt(0).Count; i++)
            {
                Dictionary<String, String> td = new Dictionary<string, string>();
                foreach (string h in headers)
                {
                    td.Add(h, excelData[h][i]);
                }
                testDatas.Add(td);
            }
            return testDatas;
        }


        public void InsertSingleRowIntoTable(string ConnectionString, string TableName, Dictionary<string, string> InsertValues)
        {
            try
            {
                string insertCommand = getInsertCommandForSingleRow(InsertValues, TableName);
                logger.Here().Information(insertCommand);
                ExecuteDMLQueries(ConnectionString, insertCommand);
            }
            catch (Exception e)
            {
                logger.Here().Information("Error while inserting the record in the table :" + TableName);
                throw e;
            }
        }

        public string getInsertCommandForSingleRow(Dictionary<string, string> InsertValues, string TableName)
        {
            StringBuilder IQuerry = new StringBuilder();
            IQuerry.Append("\n");
            IQuerry.Append("INSERT INTO ");
            IQuerry.Append(TableName);
            IQuerry.Append("(");
            for (int index = 0; index < InsertValues.Keys.Count; index++)
            {
                IQuerry.Append(InsertValues.Keys.ElementAt(index));
                if (index != InsertValues.Count - 1)
                    IQuerry.Append(", ");
            }
            IQuerry.Append(")");
            IQuerry.Append(" VALUES(");
            for (int index = 0; index < InsertValues.Values.Count; index++)
            {
                IQuerry.Append(InsertValues.Values.ElementAt(index));

                if (index != InsertValues.Count - 1)
                    IQuerry.Append(", ");
            }
            IQuerry.Append(");");
            return IQuerry.ToString();
        }

    }
}
