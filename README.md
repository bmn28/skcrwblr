# skcrwblr
A Windows program for listening to [KCRW](http://kcrw.com), the greatest radio station in the world, and scrobbling the currently playing song to [Last.fm](http://last.fm).

Download it from the [releases](https://github.com/bmn28/skcrwblr/releases).

Requires the .NET Framework 4.5.1.

Uses the [NAudio](https://naudio.codeplex.com/) open source audio library under the [Microsoft Public License](https://opensource.org/licenses/MS-PL).

This application is not affiliated with KCRW.

The GitHub repository is missing a class `ApiCredentials.cs` containing strings `ApiKey` and `Secret`, which are credentials for the [Last.fm API](http://www.last.fm/api). If you want to build from the source, you'll need to get your own credentials and recreate the class.
