# TFS Query

## Summary

This is a small console application that pulls user check in history out of a TFS instance. Admittedly check in count is not a very good metric of a user's contribution to a codebase - some developers prefer frequent small commits, while others prefer less frequent but more complete commits. Still, it's interesting information to look at.

## Libraries Used

* [Microsoft.TeamFoundation.All](https://www.nuget.org/packages/Microsoft.TeamFoundation.All/ "Microsoft.TeamFoundation.All")
* [Microsoft.TeamFoundation.VersionControl.All](https://www.nuget.org/packages/Microsoft.TeamFoundation.VersionControl.All/ "Microsoft.TeamFoundation.VersionControl.All")

## Usage

Make sure to fill out the TFS server address and a list of pipe-delimited Active Directory domains to whitelist in the `App.Config` file, and run the utility.
