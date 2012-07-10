#! /bin/perl
#
#                          Log File Watcher
# 
#                  by John Walker -- November 1997
#                      http://www.fourmilab.ch/
#
#               This program is in the public domain.

    $version = '1.1';
    $reldate = '2002 August 7';
#
#   Long-term survivors in the system administration game know that
#   an excellent way to spot little emerging problems before they
#   grow up into moose-sized disasters is keeping an eye on the
#   various system log files.  In days of yore, this was quite
#   simple, since almost everything of interest went into the
#   master Unix logfile and, if a networked system supported the
#   "loghost" facility, logs for a number of machines on the
#   network could be archived centrally.

#   As more and more services have come into common use, log files
#   have proliferated.  Today, an administrator might want to keep
#   an eye on:

#           Master system log
#           FTP transfer log
#           Web (HTTP) server access log
#           UPS power incident log
#           Network Time Protocol synchronisation log
#           Automated backup system log
#           Firewall suspicious access log
#           Logs from local services such as Speak Freely
#               Look Who's Listening and Echo servers, etc.

#   and a host of others.  Many of the widely-used daemons for these
#   functions do not use the common Unix log mechanism, and insist
#   on appending entries to their own private file.  In the case
#   of commercial firewall, backup, and UPS daemons, source code is
#   not usually available, so modifying them is not an option.

#   When there was a single log file, monitoring it couldn't be
#   simpler--just use:

#       tail -f /var/adm/messages

#   (or whatever the file name your system uses).  The -f option causes
#   tail to start at the end of the file and periodically monitor it
#   for growth, printing any additions to the file.  Unfortunately,
#   tail will only monitor a single file at a time, so the only way
#   to monitor multiple files was to launch separate jobs running
#   tail on each individual file.  Yuck.  Worse, to prevent log files
#   from growing without bound and consuming all the available disc
#   space, many programs that write logs provide a mechanism,
#   automatic or semi-automatic, to periodically cycle the log
#   to a new file, renaming the previous log file.  Unfortunately,
#   "tail -f" does not detect that this has happened, and just waits
#   forever at the end of the old log, not knowing that a new file is
#   now in use.  If servers are run on a number of different machines
#   (or you simply want to watch the Unix log files on all the hosts
#   you're responsible for to keep an eye out for developing disc
#   problems, etc.), you would then have to rsh copies of "tail -f"
#   on each machine you wanted to monitor, and then make sure the
#   process got restarted when a machine running it rebooted.

#   This program allows you to monitor any number of log files on
#   any number of machines (assuming they support Perl with
#   networking and use ASCII log files).  Logs from any machine can
#   be echoed to as many other machines as you like.  Log files
#   being monitored are closed and re-opened at a user-defined
#   interval, so that cycling of log files into new files is
#   followed automatically.

#   The following definitions are correct for most System V
#   compliant versions of Unix.  Older releases, particularly
#   those derived from BSD, may require different settings.
#   You can determine the correct values for your system by
#   examining the file /usr/include/sys/socket.h or by compiling
#   and running the following C program, which prints the correct
#   declarations for your system.
#
#   #include <stdio.h>
#   #include <sys/types.h>
#   #include <sys/socket.h>
#   main() {
#       printf("    $AF_INET = %d;\n    $SOCK_DGRAM = %d;\n", AF_INET, SOCK_DGRAM);
#       return 0;
#   }
#
#
#   Release History
#
#   Version 1.0: 1997 November 7th
#   	Initial release.
#
#   Version 1.1: 2002 August 7th
#
#   	Control chsaracters embedded in log file entries could change terminal
#   	window modes and otherwise screw things up.  Added code to change all
#   	non end of line control characters to spaces.
#
#   	Added version, release date, and contact information to -u option
#   	display.

    $AF_INET = 2;
    $SOCK_DGRAM = 1;
    $WNOHANG = 0100;

    #   Default settings when options not specified

    $defaultPort = 5741;
    $defaultReopen = 15;

    $reopenTime = $defaultReopen * 60; # How often to re-open files
    $sleepTime = 1;                   # Sleep time between poll of files
    $listen = 0;                      # Port to listen on
    $dnsport = $defaultPort + 1;      # Port for DNS information from forked processes
    $talk = $defaultPort;             # Port to echo to
    $quiet = 0;                       # Don't print locally if true
    $resolve = 0;                     # Resolve IP addresses in messages ?
    $forking = 1;                     # Fork to resolve IP addresses ?
    $dnstimeout = 10 * 60;            # Purge DNS entry after this number of seconds of inactivity
    $verbose = 0;                     # Print debug output if true
    $domains = 0;                     # Show domain name on echoes received ?
    $lineWrap = 79;                   # Wrap lines at this column
    $lineBreak1 = '[,&]';             # Line break first pass candidates
    $lineBreak2 = '[/+]';             # Line break second pass candidates

    #   Optimisation parameters

    $REDNS_empty_slots = 100;         # Minimum empty slots to rebuild DNS cache
    $REDNS_empty_fraction = 50;       # Fraction empty slots to force debuild DNS cache

    #   Internal debugging flags

    $progress = 0;                    # Show progress if nonzero

    #   Definitions for network access

    $sockaddr = 'S n a4 x8';

    #   Process options on command line

    $echoes = 0;                      # Number of echo destinations
    for ($i = 0; $i <= $#ARGV; $i++) {
        if ($ARGV[$i] =~ m/^-/) {
            $o = $ARGV[$i];
            splice(@ARGV, $i, 1);
            $i--;
            if (length($o) == 1) {
                last;
            }
            $opt = substr($o, 1, 1);
            $arg = substr($o, 2);

            #   -d                  --  Show full domain name for echoes

            if ($opt eq 'd') {
                $domains = 1;

            #   -ehostname          --  Echo to hostname

            } elsif ($opt eq 'e') {
                local($destaddr);

                ($d1, $d2, $d3, $d4, @destaddr) = gethostbyname($arg);
                if (!defined @destaddr) {
                    die("Invalid echo host name: $arg\n");
                }
                push(@destinations, $destaddr[0]);
                $echoes++;

            #   -lport              -- Listen for echo on given port

            } elsif ($opt eq 'l') {
                if (length($arg) > 0) {
                    if ($arg =~ m/^\d+$/) {
                        $listen = $arg;
                    } else {
                        die("Invalid port number '$arg' in -l option.\n");
                    }
                } else {
                    $listen = $defaultPort;
                }
                $dnsport = $listen + 1;

            #   -n                  -- No forking for DNS lookups.  Run
            #                          serially in main process.

            } elsif ($opt eq 'n') {
                $forking = 0;

            #   -phowmany           -- Reopen log files every howmany
            #                          minutes, 0 = never

            } elsif ($opt eq 'p') {
                if ($arg =~ m/^\d+$/ && $arg >= 0) {
                    $reopenTime = $arg * 60;
                } else {
                    die("Invalid reopen time '$arg' in -p option.\n");
                }

            #   -q                  -- Quiet: don't print locally, just echo

            } elsif ($opt eq 'q') {
                $quiet = 1;

            #   -rtime              -- Resolve: try gethostbyaddr for IP addresses
            #                          Use time (seconds) for DNS cache timeout.

            } elsif ($opt eq 'r') {
                $resolve = 1;
                if (length($arg) > 0) {
                    if ($arg =~ m/^\d+$/) {
                        $dnstimeout = $arg;
                    } else {
                        die("Invalid DNS timeout '$arg' in -r option.\n");
                    }
                }

            #   -stime              -- Sleep time seconds between polls
            #                          of log files

            } elsif ($opt eq 's') {
                if ($arg =~ m/^\d+$/ && $arg > 0) {
                    $sleepTime = $arg;
                } else {
                    die("Invalid sleep time '$arg' in -s option.\n");
                }


            #   -tport              -- Transmit echo on given port

            } elsif ($opt eq 't') {
                if ($arg =~ m/^\d+$/) {
                    $talk = $arg;
                } else {
                    die("Invalid port number '$arg' in -t option.\n");
                }

            #   -u                  -- Print how-to-call information

            } elsif ($opt eq 'u' || $opt eq '?') {
                print("Usage: logtail [ options ] logfile ...\n");
		print("Version $version -- $reldate.\n");
                print("       Options:\n");
                print("             -d              Show full domain name for echo messages.\n");
                print("             -ehostname      Echo to named host.  Multiple -e options\n");
                print("                             echo to multiple hosts.\n");
                print("             -lport          Listen for echo on port (default $defaultPort),\n");
                print("                             0 = don't listen.\n");
                print("             -n              No forking for -r DNS lookups\n");
                print("             -pinterval      Reopen log files every interval\n");
                print("                             minutes (default $defaultReopen), 0 = never.\n");
                print("             -q              Quiet: don't print on local machine, just echo.\n");
                print("             -rtime          Resolve: attempt to find host names from IP\n");
                print("                             addresses in the log.  Cache timeout: time secs.\n");
                print("             -stime          Sleep time seconds between polls of files.\n");
                print("             -tport          Transmit echo on port (default $defaultPort).\n");
                print("             -u              Print this message.\n");
                print("             -vlevel         Verbose: generate debug output.\n");
                print("             -wcols          Wrap lines at cols columns, 0 = no wrap.\n");
		print("The latest version of this program is available\n");
		print("from: http:/www.fourmilab.ch/webtools/logtail/\n");
		print("Report bugs to bugs\@fourmilab.ch\n");
                exit(0);

            #   -vlevel             -- Verbose: generate debug output

            } elsif ($opt eq 'v') {
                $verbose = 1;
                if (length($arg) > 0) {
                    if ($arg =~ m/^\d+$/ && $arg >= 0) {
                        $verbose = $arg;
                    } else {
                        die("Invalid verbosity level '$arg' in -v option.\n");
                    }
                }
                if ($verbose > 1) {
                    if ($verbose >= 2) {
                        $progress = 1;
                    }
                    $verbose = 1;
                }

            #   -wcols              -- Wrap lines at cols columns, 0 = no wrap

            } elsif ($opt eq 'w') {
                if ($arg =~ m/^\d+$/ && $arg >= 0) {
                    $lineWrap = $arg;
                    if ($lineWrap == 0) {
                        $lineWrap = 1 << 31;
                    }
                } else {
                    die("Invalid wrap length '$arg' in -w option.\n");
                }
            }
        }
    }

    if ($progress) {
        print("Main process $$\n");
    }

    #   If we're to receive messages from other hosts, create
    #   the inbound socket and bind it to the specified port.

    if ($listen) {
        $sock = pack($sockaddr, $AF_INET, $listen, "\0\0\0\0");
        socket(SOCKIN, $AF_INET, $SOCK_DGRAM, 0) || die "Error creating socket: $!";
        bind(SOCKIN, $sock) || die "Error binding socket: $!";
        select(SOCKIN);
        $| = 1;
        select(STDOUT);
        vec($insock, fileno(SOCKIN), 1) = 1;
    }

    #   If we're going to use multiple processes to resolve
    #   IP address lookups, assemble the necessary machinery
    #   to manage the processes and communication between them.

    if ($forking) {
        $| = 1;                       # Force output unbuffered
        $dnsock = pack($sockaddr, $AF_INET, $dnsport, "\0\0\0\0");
        socket(SOCKDNS, $AF_INET, $SOCK_DGRAM, 0) || die "Error creating socket: $!";
        bind(SOCKDNS, $dnsock) || die "Error binding socket: $!";
        select(SOCKDNS);
        $| = 1;
        vec($dnsocks, fileno(SOCKDNS), 1) = 1;
        $dnshost = pack($sockaddr, $AF_INET, $dnsport, pack('C4', 127, 0, 0, 1));
        select(STDOUT);
    }

    #   If we're echoing to other hosts, create the destination
    #   addresses for them.  We do this here, as opposed to in
    #   the "e" option handler, so that all will use the port
    #   number given by the "-t" option.

    if ($echoes) {
        socket(SOCKOUT, $AF_INET, $SOCK_DGRAM, 0) ||
            die("Cannot create outbound echo socket: $!");
    }
    for ($i = 0; $i < $echoes; $i++) {
        $destinations[$i] = pack($sockaddr, $AF_INET, $talk, $destinations[$i]);
    }

    $utime = 0;

    #   This is the main processing loop

    while (1) {

        #   If $reopenTime has elapsed since the last refresh, flag a
        #   reopen required.  Reopening allows us to track files which
        #   are automatically cycled with the previous file being
        #   renamed.

        $reopen = ($reopenTime > 0) && ((time() - $utime) >= $reopenTime);

        $grew = 0;
        foreach $i (@ARGV) {
            if ($utime != 0) {
                if ($progress) {
                    print("++++ $$ Checking file $i\n");
                }
                $s = -s $i;
                if (defined $s) {
                    if ($sizes{$i} < $s) {
                        if ($progress) {
                            printf("++++ $$ Reading %d bytes from file $i\n", $s - $sizes{$i});
                        }
                        $nread = read($i, $t, $s - $sizes{$i});

                        #   The following if block merits a little
                        #   explanation.  Since items are appended
                        #   to the log file by another independent
                        #   process, it is possible (albeit unlikely)
                        #   that when we go to read the file the
                        #   other process may not have finished writing
                        #   the log item.  In this case we'll receive a
                        #   partial item, which may not be terminated
                        #   by a new line character.  Such an item could
                        #   mess up our output and confuse the code which
                        #   splits blocks we read into lines.  Since this
                        #   happens so rarely, and should fix itself
                        #   by the next time we poll for updates, we
                        #   take the easy way out and just skip the file
                        #   on this poll, counting on a complete read the
                        #   next time.  It would be more general to
                        #   process any complete lines in the block
                        #   and advance the pointer to the end of
                        #   the last complete line, but for the sake
                        #   of simplicity, we just re-seek the file
                        #   to the start of this read so it's re-read on
                        #   the next poll.

                        if (!($t =~ m/.*\n$/)) {
                            seek($i, $sizes{$i}, 0);
                            if ($progress) {
                                print("*** Setting up to reread $i starting at $sizes{$i}\n");
                            }
                        } else {
                            $sizes{$i} = $s;
                            $grew++;
                            #   Print locally and echo to specified hosts
                            &deliver($t, 1);
                        }
                    }
                }
            }

            #   If it's time to reopen the files, do it for this one

            if ($reopen) {
                &reopenFile($i);
            }
        }
        if ($reopen) {
            $utime = time();
        }
        if ($progress) {
            printf("++++ $$ Done scanning files.\n");
        }

        #   Now see if echo transmissions are queued from
        #   one or more other hosts.  If so, echo them,
        #   prefixed with the host name.  Note that echoed
        #   messages received are not echoed to other hosts.
        #   This not only prevents ugly accidental echo loops,
        #   it allows a number of peers to echo to each other,
        #   so the status of all is visible on any.  Further,
        #   it permits echoing to the same host, which is very
        #   handy for debugging.  By using &deliver to display
        #   the output, we allow host name resolution to be done
        #   on echoed material.  In some environments (for example,
        #   a heavily-loaded mission-critical server being monitored
        #   by a hideously overpowered administrator's workstation)
        #   it makes sense to forward unresolved logs to the machine
        #   on which they're watched and do the resolution there.
        #   Any resolution done on the sending machine will not,
        #   however, be repeated by the recipient of the echo.

        if ($listen && $progress) {
            print("++++ $$ Checking echo\n");
        }
        while ($listen && select($d1 = $insock, undef, undef, 0)) {
            $sender = recv(SOCKIN, $msg, 65535, 0);
            $sender = substr($sender, 4, 4);
            $d1 = $dns{$sender};
            if (!defined $d1) {
                undef($name);
                $name = gethostbyaddr($sender, $AF_INET);
                if (defined $name) {
                    if (!$domains) {
                        $name =~ s/\..*$//;
                    }
                } else {
                    ($b1, $b2, $b3, $b4) = unpack('C4', $sender);
                    $name = "$b1.$b2.$b3.$b4";
                }
                $dns{$sender} = $name;
            }
            $d1 = $dns{$sender};
            $msg =~ s/\n$//;
            $msg =~ s/\n/\n$d1: /g;
            #   Print the message locally, but don't echo
            &deliver("$d1: $msg\n", 0);
        }

        #   Reap any child processes which have gone to seed since the
        #   last time around the loop.

        if ($resolve && $forking) {
            if ($progress) {
                print("++++ $$ Reaping\n");
            }
            while (1) {
                $pid = waitpid(-1, $WNOHANG);
                if ($verbose) {
                    print "   Reaped process $pid\n";
                }
                last if ($pid < 1);
                $forkCount--;
            }

            if ($forkCount != $lFC) {
                if ($progress) {
                    if ((time() - $RPLast) > 60) {
                        $RPLast = time();
                        print("** Resolve processes: $forkCount\n");
                        if ($DNS_total > 0) {
                            $DNS_cacheper = int((100 * ($DNS_total - $DNS_lookup)) / $DNS_total);
                            print("** DNS lookups: $DNS_total, $DNS_cacheper% found in cache.\n");
                        }
                    }
                }
                $lFC = $forkCount;
            }
        }

        #   Time out and remove DNS entries for hosts we haven't
        #   heard from recently.

        if ($resolve) {
            if ($progress) {
                print("++++ $$ Checking for DNS timeout\n");
            }
            if ((time() - $lpLast) >= ($dnstimeout / 2)) {
                $lpLast = time();
                $zzundef = 0;
                $zzslots = 0;
                while (($zzx, $zzxv) = each(%rdnsLastref)) {
                    $zzslots++;
                    if (defined $rdnsLastref{$zzx}) {
                        $xx01 = $lpLast - $rdnsLastref{$zzx};
                        if ($xx01 > $dnstimeout) {
                            undef($rdnsLastref{$zzx});
                            undef($rdns{$zzx});
                        }
                    } else { 
                        $zzundef++;
                    }
                }
                if ($progress) {
                    print("$zzslots slots in DNS hash, $zzundef undefined.\n");
                }
                if (($zzundef > $REDNS_empty_slots)  &&
                    (((100 * $zzundef) / $zzslots) > $REDNS_empty_fraction)) {
                    local(%NEWrdns, %NEWrdnsLastref);

                    #   Even though we undefined timed-out DNS hash
                    #   items, the space in the hash table itself is
                    #   not reclaimed and tends to grow without bound.
                    #   When things reach a the thresholds of
                    #   at least $REDNS_empty_slots and the array
                    #   consists of $REDNS_empty_fraction percent unused
                    #   slots, we copy all the defined items to a new
                    #   array, swap it into use, and delete the old
                    #   array to reclaim its has table space.

                    if ($progress) {
                        print("Purge DNS hash: zzundef = $zzundef, zzslots = $zzslots\n");
                    }

                    while (($zzx, $zzxv) = each(%rdnsLastref)) {
                        if (defined $rdnsLastref{$zzx}) {
                            $NEWrdns{$zzx} = $rdns{$zzx};
                            $NEWrdnsLastref{$zzx} = $rdnsLastref{$zzx};
                        }
                    }
                    %rdns = %NEWrdns;
                    %rdnsLastref = %NEWrdnsLastref;
                }
            }
        }

        if ($progress) {
            print("++++ $$ Sleeping\n");
        }
        sleep($sleepTime);
        if ($progress) {
            print("++++ $$ Done sleeping\n");
        }
        &checkDNS();
    }

#   deliver  --  Resolve the block, then deliver it to the
#                requested destinations.

sub deliver {
    local($s, $doEcho) = @_;
    local($forked, $h, $d1);
    
    #	Change any control or undefined ISO characters into spaces
    #	lest they screw up terminal modes, etc.  We have to leave
    #	end of line characters intact so the line breaking
    #	code will work.
    $s =~ s/[\000-\011\013-\014\016-\037\200-\240]/ /g;

    $forked = 0;

    #   If host name resolution is requested, scan the block
    #   for any sequences which look like IP addresses and
    #   pass them to gethostbyaddr to look for the corresponding
    #   fully qualified domain name.  If the name is found, it is
    #   cached in our private $rdns{} hash to avoid further
    #   lookups.  If the name lookup fails, a numeric IP
    #   address is used instead of the name.

    if ($resolve) {
        $l = &resfork($s);
        if ($l > 0) {
            return;
        } elsif ($l == 0) {
            $forked = 1;
        }
        $s = &resolution($s, $forked);
    }

    #   CAUTION!  Below this point we may be running in a child
    #   processed forked for parallel resolution.  From this point
    #   on, any changes to global variables or program state will
    #   be seen by the main process.  Be sure to make all such
    #   changes before the "if ($resolve)" loop above, and take
    #   care that no function you call from now on itself modifies
    #   global state.

    #   Unless quiet mode is on, print the message block on
    #   standard output.

    if (!$quiet) {
        if ($progress) {
            printf("++++ $$ Going to print\n");
        }
        &printWrap($s);
        if ($progress) {
            printf("++++ $$ Done printing\n");
        }
    }

    #   If we're echoing to one or more other hosts,
    #   send a copy of the information to each.

    if ($doEcho && $echoes) {
        if ($progress) {
            printf("++++ $$ Sending echoes\n");
        }
        foreach $h (@destinations) {
            $d1 = send(SOCKOUT, $s, 0, $h);
            if (!defined $d1) {
                print("Error writing to echo socket: $!\n");
            }
        }
        if ($progress) {
            printf("++++ $$ Done sending echoes\n");
        }
    }

    #   If this is a child process, our incarnation in the main process
    #   has already returned.  So, there's nothing left for us but to exit.

    if ($forked) {
        exit(0);
    } 
}

#   reopenFile  --  Close (if already open) and reopen a file
#                   we're monitoring.  This transfers our scan to
#                   new instances when logging is cycled from one
#                   physical file to another.

sub reopenFile {
    local($i) = $_[0];
    local($s);

    if ($progress) {
        printf("++++ $$ Closing and reopening file $i.\n");
    }
    if ($utime != 0) {
        close($i);
    }
    if (open($i, "<$i")) {
        #   *** Check if inode of old file is same as new file.  If not,
        #       start reading at zero, not the last seek address.  Note
        #       that reading should be line by line so as not to blow the
        #       65536 MTU of the socket if we're echoing.
        $s = -s $i;
        $sizes{$i} = $s;
        seek($i, $s, 0);
        if ($verbose) {
            print("** Opened $i, size $s\n");
        }
    } else {
        die("** Warning: cannot open file $i: $!\n");
    }
}

#   printWrap  --  Print one or more lines with wrap at
#                  the specified column.

sub printWrap {
    local($s) = $_[0];
    local($l, $sep, $rem, $ter, $lwrap);

    #   Pick the input apart line by line and reformat each line,
    #   if necessary, so as not to exceed the maximum line length.
    #   Because we may be running as a subprocess, the wrapping
    #   of each line assembled a string containing all the resulting
    #   lines, which can be written by an atomic I/O, guaranteeing
    #   portions of different lines won't be interleaved due to
    #   switching among multiple processes.

    while (length($s) > 0) {
        if (($s =~ s/(.*\n)//) != 1) {
            $aax = $_[0];
            print("printWrap arg = |$aax|\n");
            print("printWrap s = |$s|\n");
            $aal = length($s);
            print("printWrap length(s) = $aal\n");
            die("Error splitting lines.");
        }
        $l = $1;

        $sep = '';
        $lwrap = '';
        while (length($l) > $lineWrap) {
            if (($l =~ s/(^.{1,$lineWrap})(\s)//o) || 
                ($l =~ s/(^.{1,$lineWrap})($lineBreak1)//o) ||
                ($l =~ s/(^.{1,$lineWrap})($lineBreak2)//o)
               ) {
                $rem = $1;
                $ter = $2;
                if ($ter =~ m/\s+/) {
                    $ter='';
                }
                $lwrap .= "$sep$rem$ter\n";
                $l =~ s/^\s*//;
                $sep = "        ";
            } else {
                last;
            }
        }
        print("$lwrap$sep$l");
    }
}

#   &resfork  --  This subroutine examines its argument to determine
#                 if it contains any numeric IP address which aren't
#                 in the %rdns{} cache.  If so, and $forking is
#                 defined, it creates a child process to perform
#                 the resolution.  In this case it returns to the
#                 caller with the PID of the child process and
#                 to the caller in the child process with zero.
#                 If no fork was required, it simply returns with
#                 zero.  In general, the caller of &resfork should
#                 immediately return to its own caller when &resfork
#                 returns < zero, and continue processing (e.g.
#                 calling &resolution) on a zero return.

sub resfork {
    local($s) = $_[0];
    local($l, $tail, $ip);
    local($name, $child);

    $tail = $s;
    &checkDNS();

    #   If we're doing DNS lookups in separate processes, prescan
    #   the message to see if it contains any IP addresses not
    #   already in the cache.  If so, fork an auxiliary process
    #   to perform the lookups.  If we spawn a process, it will
    #   have $forked set to 1, which informs it to relay the
    #   results of any host lookups to the main process via the
    #   DNS socket.  When we fork a process, the parent process
    #   returns immediately from &printWrap() while the child
    #   remains to complete the resolution and print the results.

    if ($forking) {
        while ($tail =~ m/(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})/ &&
            $1 <= 255 && $2 <= 255 && $3 <= 255 && $4 <= 255) {
            $tail = $';
            $ip = "$1.$2.$3.$4";
            &checkDNS();
            $DNS_total++;
            if (defined($name = $rdns{$ip})) {
                #   Update time of last reference to $rdns entry
                $rdnsLastref{$ip} = time();
            } else {
                $DNS_lookup++;
                while (1) {
                    if ($child = fork()) {
                        $forkCount++;
                        if ($verbose) {
                            print("$s");
                            print("Looking up $ip in process $child.\n");
                        }
                        return $child;
                    } elsif (defined $child) {
                        # We are now running in the child process
                        close(SOCKDNS);
                        close(SOCKIN);
                        close(SOCKOUT);
                        socket(SOCKOUT, $AF_INET, $SOCK_DGRAM, 0) ||
                            die("Cannot create outbound echo socket in child process $$: $!");
                        socket(SOCKDNSOUT, $AF_INET, $SOCK_DGRAM, 0) ||
                            die("Cannot create outbound DNS socket in child process $$: $!");
                        select(SOCKDNSOUT);
                        $| = 1;
                        select(STDOUT);
                        $forked = 1;
                        return 0;
                    } elsif ($! =~ /No more process/) {
                        sleep(2);
                    } else {
                        if ($verbose) {
                            print("** Cannot fork to look up $ip: $!\n");
                        }
#                       die("Fork failed: $!\n");
                        return -1;  # Do lookup in main process
                    }
                }
            }
        }
    }
    return -1;
}

#   &resolution  --  "You say you want a res-o-lution, well, you
#                    you, you'd better use this code, my friend."
#                    This subroutine attempts to replace any numeric
#                    IP address in its argument with the corresponding
#                    host names obtained with gethostbyaddr.  The second
#                    argument is zero when called from the main process
#                    and nonzero when called from a child process.  This
#                    information determines whether additions to the DNS
#                    cache are done directly or related to the main process
#                    over the DNS socket.
#

sub resolution {
    local($s, $forked) = @_;
    local($head, $tail, $ip, $bip);
    local($name, $flookup);


    #   Now walk through the string and resolve any candidate IP
    #   addresses.  The apparent duplication of the code in &resfork
    #   is because in the first scan we don't know whether we'll be
    #   executing this function in the main process or in a forked
    #   child process.

    $tail = $s;
    $s = '';
    while ($tail =~ m/(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})/ &&
        $1 <= 255 && $2 <= 255 && $3 <= 255 && $4 <= 255) {
        $head = $`;
        $tail = $';
        $ip = "$1.$2.$3.$4";
        $bip = pack('C4', $1, $2, $3, $4);

        if (!defined($name = $rdns{$ip})) {
            undef($name);
            $name = gethostbyaddr($bip, $AF_INET);
            if (!defined $name) {
                $name = $ip;
            }

            #   Update our own private $rdns{} so subsequent references
            #   within this sequence of entries will be resolved from the
            #   cache.  If we're not a forked process, this is all we
            #   need to do to update the cache.

            $rdns{$ip} = $name;
            $rdnsLastref{$ip} = time();

            if ($forked) {

                #   Now send a packet containing the IP address and the host
                #   name it resolved to (space delimited) to the DNS socket
                #   of the main process.

                $flookup .= "$ip $name ";
                $d1 = send(SOCKDNSOUT, $flookup, 0, $dnshost);
                if (!defined $d1) {
                    print("Error writing DNS lookup to port $dnsport socket: $!\n");
                }
            }
            if ($progress) {
                $name = "!$name";
            }
        }
        $s .= $head . $name;
    }
    $s .= $tail;

    return $s;
}

#   &checkDNS  --  See if any new DNS messages have arrived.  If so,
#                  process them.

sub checkDNS {
    local ($msg, $d1);

    while ($forking && select($d1 = $dnsocks, undef, undef, 0)) {
        recv(SOCKDNS, $msg, 65535, 0);
        if ($verbose) {
            print("DNS lookup message received: $msg\n");
        }
        #   Now add the lookups to our DNS database
        while ($msg =~ m/(^\S+)\s(\S+)\s/) {
            $msg = $';
            $rdns{$1} = $2;
            $rdnsLastref{$1} = time();
        }
    }
}
