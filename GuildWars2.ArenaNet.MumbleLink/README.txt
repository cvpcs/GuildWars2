Guild Wars 2 Mumble Link program
Version 1.0

== INSTALLATION ===============================================================

There is no installation for this program. You just double-click it and let it
do it's thing!

The only requirement is that you have .NET 4.0 or later installed. If you do
not you can download it from Microsoft here:

http://www.microsoft.com/en-us/download/details.aspx?id=17851

== USAGE ======================================================================

To use the program, simply double-click on it and let it run. The program is
command-line based, so it will open in a simple text terminal.

Leave this program running in the background while you play Guild Wars 2 to
allow other programs to access your positional data.

To close the program, simply focus on the window it is running in and press
any key.

== HOW IT WORKS ===============================================================

Guild Wars 2 has native support for the positional audio features within the
Mumble voice chat program. What this means is that when Guild Wars 2 starts, it
creates a readable portion of memory on your computer that it stores
information such as your character name, server, map, and location within that
map (among other things). When running Mumble, this information is used to
alter how voice communications are sent to the user in order to make players
that are to your left in-game sound as if they are to your left through your
speakers.

The Mumble Link program creates/reads from that same location in memory that
mumble usually would in order to ascertain your character's positional 
information. Once it has this information, it needs a way to allow other
programs (such as your browser) to read it.

It does this by creating a simple web server on your computer. This server is
completely in memory and does nothing but return your players positional data
whenever it recieves a web request.

== COMMON QUESTIONS ===========================================================

Q: You said this creates a web server. Can the interwebs access my computer if
   I run this?
A: No. The webserver only listens on localhost (a.k.a the "loopback" address).
   In non-technical terms, this means can only be reached from and respond to
   requests that originate from the computer it is running on.

Q: This program accesses memory that Guild Wars 2 is writing to? Will this get
   my account banned?
A: No. This method is indistinguishable from the interaction that the Mumble
   voice program uses. ArenaNet has been active in assisting developers in
   using this method of position tracking for several months now on the
   official API forums found here:
   https://forum-en.guildwars2.com/forum/community/api