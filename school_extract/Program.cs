using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;



namespace school_extract
{
    class Program
    {
        static void Main(string[] args)
        {
            using (
                var conn = new SqlConnection("Server=0;Database=0;User ID=0;Password= 0;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;")
                )
            {
                conn.Open();

                bool quit = false;
                string choice;
                SqlCommand cmd = new SqlCommand();
                Console.WriteLine("<---------------------");
                Console.WriteLine("        School");
                Console.WriteLine("--------------------->");

                while (!quit)
                {
                    Console.WriteLine("Select by public, private, or none?");
                    string category = Console.ReadLine();
                    if (category == "public")
                    {
                        Console.WriteLine("Select by code, date, both, or none?");
                        choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "code":
                                Console.WriteLine("Select by code1 or code2?");
                                string codeCol = Console.ReadLine();
                                Console.WriteLine("Enter desired code");
                                string code = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM School WHERE (School." + @codeCol + "='" + @code + "') AND category ='" + @category + "' FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            case "date":
                                Console.WriteLine("Enter desired date using the format MM/DD/YYYY.");
                                string date = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM School WHERE (School.date_created >= '" + @date + "' OR School.date_updated >= '" + @date + "') AND category ='" + @category + "' FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            case "both":
                                cmd = new SqlCommand("SELECT * FROM School WHERE category ='" + @category + "'FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;

                                break;

                            case "none":
                                cmd = new SqlCommand("SELECT * FROM School WHERE category ='" + @category + "' FOR XML PATH('school'), ROOT('lk12School')", conn);
                                quit = true;
                                break;

                            default:
                                Console.WriteLine("Unknown Command " + choice);
                                continue;
                        }
                    }
                    else if (category == "private")
                    {
                        Console.WriteLine("Select by code, date, both, or none?");
                        choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "code":
                                Console.WriteLine("Select by code1 or code2?");
                                string codeCol = Console.ReadLine();
                                Console.WriteLine("Enter desired code");
                                string code = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM School WHERE (School." + @codeCol + "='" + @code + "') AND category ='" + @category + "' FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            case "date":
                                Console.WriteLine("Enter desired date using the format MM/DD/YYYY.");
                                string date = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM School WHERE (School.date_created >= '" + @date + "' OR School.date_updated >= '" + @date + "') AND category ='" + @category + "' FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            case "both":
                                cmd = new SqlCommand("SELECT * FROM School WHERE category ='" + @category + "' FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;

                                break;

                            case "none":
                                cmd = new SqlCommand("SELECT * FROM School WHERE category ='" + @category + "' FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            default:
                                Console.WriteLine("Unknown Command " + choice);
                                continue;
                        }
                    }
                    else if (category == "none")
                    {
                        Console.WriteLine("Select by code, date, both, or none?");
                        choice = Console.ReadLine();
                        switch (choice)
                        {
                            case "code":
                                Console.WriteLine("Select by code1 or code2?");
                                string codeCol = Console.ReadLine();
                                Console.WriteLine("Enter desired code");
                                string code = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM School  WHERE (School." + @codeCol + "='" + @code + "') FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            case "date":
                                Console.WriteLine("Enter desired date using the format MM/DD/YYYY.");
                                string date = Console.ReadLine();
                                cmd = new SqlCommand("SELECT * FROM School WHERE (School.date_created >= '" + @date + "' OR School.date_updated >= '" + @date + "') FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            case "both":
                                cmd = new SqlCommand("SELECT * FROM School FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;

                                break;

                            case "none":
                                cmd = new SqlCommand("SELECT * FROM School FOR XML PATH('school'), ROOT('k12School')", conn);
                                quit = true;
                                break;

                            default:
                                Console.WriteLine("Unknown Command " + choice);
                                continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unknown Command " + category);
                        continue;
                    }
                }
                using (cmd)
                {
                    using (var reader = cmd.ExecuteXmlReader())
                    {
                        var doc = new XDocument();
                        try
                        {
                            doc = XDocument.Load(reader);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("There are no entries that match the given parameters.");
                        }
                        string path = @"School." + DateTime.Now.ToString("yyyyMMdd") + ".xml";
                        using (var writer = new StreamWriter(path))
                        {
                            XNamespace ns = "http://specification.sifassociation.org/Implementation/na/3.2/html/CEDS/K12/K12_k12School.html";
                            var root = new XElement(ns + "k12School");
                            int count = 0;

                            foreach (var d in doc.Descendants("school"))
                            {
                                string ncesid;
                                string privateNCESID;
                                string delete;
                                string street;
                                string street2;
                                string campus;

                                /*Campus modification*/
                                if ((string)d.Element("campus") != null)
                                {
                                    campus = "-" + (string)d.Element("campus");
                                }
                                else
                                {
                                    campus = string.Empty;
                                }

                                /*NCESID private/ public modification*/
                                if ((string)d.Element("category") == "public")
                                {
                                    ncesid = (string)d.Element("NCESID_district") + (string)d.Element("NCESID_school") + campus;
                                    privateNCESID = string.Empty;
                                }
                                else
                                {
                                    privateNCESID = (string)d.Element("NCESID_district") + (string)d.Element("NCESID_school") + campus;
                                    ncesid = string.Empty;
                                }

                                /*Delete flag modification*/
                                if ((string)d.Element("delete_flag") == "Y")
                                {
                                    delete = "1";
                                }
                                else
                                {
                                    delete = "0";
                                }

                                /*street check for null*/
                                if ((string)d.Element("streetLine2") == "NULL")
                                {
                                    street2 = string.Empty;
                                    street = ((string)d.Element("streetLine1") + street2);
                                }
                                else
                                {
                                    street2 = (string)d.Element("streetLine2");
                                    street = ((string)d.Element("streetLine1") +"," + street2);
                                }
                                count++;
                                root.Add(new XElement(ns + "school",
                                            new XElement(ns + "identification",
                                                new XElement(ns + "schoolID", (string)d.Element("schoolID")),
                                                new XElement(ns + "name", (string)d.Element("name")),
                                                new XElement(ns + "organizationType", (string)d.Element("organizationType"))
                                                ),
                                            new XElement(ns + "directory",
                                                new XElement(ns + "gradesOfferedList",
                                                    new XElement(ns + "gradesOffered", (string)d.Element("gradesOffered"))
                                                    ),
                                            new XElement(ns + "addressList",
                                                new XElement(ns + "address",
                                                    new XElement(ns + "street",
                                                        new XElement(ns + "line1", street)

                                                        ),
                                                    new XElement(ns + "city", (string)d.Element("city")),
                                                    new XElement(ns + "stateProvince", (string)d.Element("stateProvince")),
                                                    new XElement(ns + "postalCode", (string)d.Element("postalCode")),
                                                    new XElement(ns + "county", (string)d.Element("county"))
                                                    )
                                                ),
                                            new XElement(ns + "reference",
                                                new XElement(ns + "NCESID", ncesid ),
                                                new XElement(ns + "privateschoolNCESID", privateNCESID )

                                                    )
                                                    ),
                                            new XElement(ns + "delete", delete)
                                            )
                                            );
                            }
                            
                            root.Save(writer);
                            Console.WriteLine("" + count + " school records written");
                            Console.ReadLine();

                        }


                    }
                }
            }

        }
    }
}

