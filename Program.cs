using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TFSQuery
{

    public class Program
    {

        #region Variables

        private static StringBuilder _output;

        #endregion

        #region Constructors

        static Program()
        {
            _output = new StringBuilder();
        }

        #endregion

        #region Methods

        private static void Main(string[] args)
        {
            // Signal Processing Start
            Console.WriteLine("Processing - Start");

            try
            {
                // Declare Server And Domains
                Uri server = new Uri(ConfigurationManager.AppSettings["server"]);
                List<string> domains = ConfigurationManager.AppSettings["domains"]
                                                           .Split('|')
                                                           .Where(x => !string.IsNullOrWhiteSpace(x))
                                                           .Select(x => string.Concat(x.Trim(), "\\"))
                                                           .ToList();

                // Connect To Server
                TfsTeamProjectCollection tfsTeamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(server);
                VersionControlServer versionControlServer = tfsTeamProjectCollection.GetService<VersionControlServer>();

                // Record Server And Domain Information
                _output.AppendLine("Server");
                _output.AppendLine(string.Concat("* ", server.AbsoluteUri));
                _output.AppendLine();
                _output.AppendLine("Domain(s)");
                domains.ForEach(x => _output.AppendLine(string.Concat("* ", x)));
                _output.AppendLine();

                // Record Project Information
                List<TeamProject> projects = versionControlServer.GetAllTeamProjects(true).ToList();
                _output.AppendLine("Top-Level Projects");
                projects.ForEach(x => _output.AppendLine(string.Concat("* ", x.Name)));
                _output.AppendLine();

                // Get Changeset Information
                Dictionary<string, UserInformation> userInformationList = new Dictionary<string, UserInformation>();
                List<Changeset> changesets = versionControlServer.QueryHistory("$/", VersionSpec.Latest, 0, RecursionType.Full, null, null, null, Int32.MaxValue, false, false)
                                                                 .OfType<Changeset>()
                                                                 .ToList();
                foreach (Changeset changeset in changesets)
                {
                    string userName = changeset.Committer.ToLower();
                    domains.ForEach(x => userName = userName.StartsWith(x, StringComparison.CurrentCultureIgnoreCase) ? userName.Substring(x.Length) : userName);
                    if (!userName.Equals(changeset.Committer, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!userInformationList.ContainsKey(userName))
                        {
                            userInformationList.Add(userName, new UserInformation(changeset));
                        }
                        userInformationList[userName].ProcessChangeset(changeset);
                    }
                }

                // Record Changeset Information
                ProcessChangesetInformation("Changeset Information, By Name (Active Directory)", userInformationList.OrderBy(x => x.Key));
                ProcessChangesetInformation("Changeset Information, By Name (Display)", userInformationList.OrderBy(x => x.Value.DisplayName));
                ProcessChangesetInformation("Changeset Information, By Check In Date (Initial)", userInformationList.OrderBy(x => x.Value.CheckInInitial));
                ProcessChangesetInformation("Changeset Information, By Check In Date (Final)", userInformationList.OrderBy(x => x.Value.CheckInFinal));
                ProcessChangesetInformation("Changeset Information, By Check In Count", userInformationList.OrderByDescending(x => x.Value.CheckInCount));

                // Write Output
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Concat(AppDomain.CurrentDomain.FriendlyName, ".txt")), _output.ToString());

                // Signal Processing Complete
                Console.WriteLine("Processing - Complete");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                // Signal Processing Error
                Console.WriteLine("Processing - Error");
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
            }

            // Signal Program Exit
            Console.WriteLine("Press enter to exit ...");
            Console.ReadLine();
        }

        private static void ProcessChangesetInformation(string heading, IOrderedEnumerable<KeyValuePair<string, UserInformation>> userInformationList)
        {
            _output.AppendLine(heading);
            for (int i = 0; i < userInformationList.Count(); i++)
            {
                KeyValuePair<string, UserInformation> item = userInformationList.ToList()[i];
                _output.AppendLine(string.Concat("* ", item.Value.BuildRecord(i + 1, item.Key)));
            }
            _output.AppendLine();
        }

        #endregion

    }

}