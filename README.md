# TimeDoctorObfuscator
Trick Time Doctor (TimeDoctor.com) to send tampered data. E.g. disable Time Doctor screenshots, do not report internet surfing.

_**Warning:** Use it wisely. Do not cheat on your employer. Make sure you use it legally._

### Features
* Relace every screenshot taken by TimeDoctor with sweet cat wallpaper
* Replace keystrokes count and mouse moves count by random number greater than 0
* Do not report any Website browsing
* Report usage of every application as "CensoredProcess"
* Report every window title name as "Censored Window Title"
* Prevents computer from going to sleep
* Reports "idle time" as working time, even with screen locked (however... the timeline will be splitted into multiple entries of the same task)

### Quick-Start guide
 * Run TimeDoctor, go to options->ProxySettings
  * check: proxy enabled
  * type: HTTP transparent proxy
  * address: 127.0.0.1
  * port: 7878
  * apply, ***close TimeDoctor, start it again***
  
* Compile TimeDoctorObfuscator
* Run TimeDoctorObfuscator - first time it will ask you to trust FiddlerCore Certificate - click YES every time. It is crucial to decrypt HTTPS connection between TimeDoctor.exe and TimeDoctor.com website and perform MITM attack.

Effect: TimeDoctor can only connect through TimeDoctorObfuscator. No running TimeDoctorObfuscator program equals no statistics reported. ***Always run TimeDoctorObfuscator along with TimeDoctor***

### Popups & TimeDoctor behaviour
TimeDoctor will work as normal, therefore popups like _Are you working on XXX?_ will still be there. However, that doesn't mean that it will report you chatting with friend on Facebook. Just click you are working and everything will be fine.

Inactivity is reported as unedited work time, however the timeline will be splitted into multiple entries of the same task. After a longer break, you will still need to answer question "Was you working or on a break?". If you click "I was working" - the time will be reported as unedited work time. If you click "I was on a break" - the time won't be reported as work time.

### Issues
 - Inactivity is reported as unedited work time, but the timeline reveals multiple entries for the same task.
 
### License
See LICENSE.md file.
