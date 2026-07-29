# Brute Force Blocker

A simple C# tool that monitors a text log file and automatically blocks IP addresses in Windows Firewall after 5 failed login attempts.

---

## How to Run It

1. Open Command Prompt as Administrator:
   - Search for "cmd" in Windows Start menu.
   - Right-click Command Prompt and select "Run as administrator".

2. Navigate to project folder:
   cd "C:\Users\User\Desktop\brute force blocker"

3. Start the program:
   dotnet run

---

## How to Test It

1. Open auth.txt (created automatically in the project folder) using Notepad.
2. Add this line 5 times (on separate lines):
   Failed login attempt from 192.168.1.100
3. Save the file (Ctrl + S).
4. Watch the terminal—it will detect the attempts and block the IP in Windows Firewall.

---

## How to Remove Test Firewall Rule

Run this command in Administrator Command Prompt to delete the test block rule:
netsh advfirewall firewall delete rule name="Block_192.168.1.100"
