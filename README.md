# TimeDoctorObfuscator
Trick Time Doctor (TimeDoctor.com) to send tampered data. E.g. disable Time Doctor screenshots, do not report internet surfing.

### Features
* Relace every screenshot taken by TimeDoctor with sweet cat wallpaper
* Replace keystrokes count and mouse moves count by random number greater than 0
* Do not report any Website browsing
* Report usage of every application as "CensoredProcess"
* Report every window title name as "Censored Window Title"
* Prevents computer from going to sleep

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

### Issues
 - Time raported during locked screen will still be visible in TimeDoctor.com as **Edited Time**
 - Inactivity must me still carried out
 
### License
See LICENSE.md file.
