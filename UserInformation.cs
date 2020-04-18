using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TFSQuery
{

    public class UserInformation
    {

        #region Constructors

        public UserInformation(Changeset changeset)
        {
            DisplayName = NormalizeDisplayName(changeset);
            CheckInInitial = DateTime.MaxValue;
            CheckInFinal = DateTime.MinValue;
            CheckInCount = 0;
        }

        #endregion

        #region Properties

        public string DisplayName { get; set; }

        public DateTime CheckInInitial { get; set; }

        public DateTime CheckInFinal { get; set; }

        public int CheckInCount { get; set; }

        #endregion

        #region Methods

        private static string NormalizeDisplayName(Changeset changeset)
        {
            List<string> displayNameComponents = changeset.CommitterDisplayName.Split(',').ToList();
            if (displayNameComponents.Count == 2)
            {
                return string.Format("{0} {1}", displayNameComponents[1].Trim(), displayNameComponents[0].Trim());
            }
            else
            {
                return changeset.CommitterDisplayName;
            }
        }

        public void ProcessChangeset(Changeset changeset)
        {
            CheckInCount += 1;
            if (changeset.CreationDate < CheckInInitial)
            {
                CheckInInitial = changeset.CreationDate;
            }
            if (changeset.CreationDate > CheckInFinal)
            {
                CheckInFinal = changeset.CreationDate;
            }
        }

        public string BuildRecord(int id, string userName)
        {
            return string.Format("{0}) {1} | {2} | {3} Total Changeset(s) | {4} to {5}", id.ToString().PadRight(2, ' '),
                                                                                         userName.PadRight(15, ' '),
                                                                                         DisplayName.PadRight(25, ' '),
                                                                                         CheckInCount.ToString().PadRight(4, ' '),
                                                                                         CheckInInitial.ToString("yyyy-MM-dd"),
                                                                                         CheckInFinal.ToString("yyyy-MM-dd"));
        }

        #endregion

    }

}