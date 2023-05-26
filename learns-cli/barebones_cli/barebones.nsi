!include "MUI.nsh"

!define MUI_ABORTWARNING # This will warn the user if they exit from the installer.

!insertmacro MUI_PAGE_WELCOME # Welcome to the installer page.
!insertmacro MUI_PAGE_LICENSE "C:\Users\kmedr\OneDrive\Desktop\backends-projects\Utils\barebones\license.txt"
!insertmacro MUI_PAGE_DIRECTORY # In which folder install page.
!insertmacro MUI_PAGE_INSTFILES # Installing page.
!insertmacro MUI_PAGE_FINISH # Finished installation page.

!insertmacro MUI_LANGUAGE "English"

Name "BareBones by MichaelR.Dev" # Name of the installer (usually the name of the application to install)

# define name of installer
OutFile "barebones_installer.exe"
 
# define installation directory
InstallDir $PROGRAMFILES\BareBones

ShowInstDetails show # This will always show the installation details.
 
# For removing Start Menu shortcut in Windows 7
RequestExecutionLevel admin
 
# start default section
Section
 
    # set the installation directory as the destination for the following actions
    SetOutPath $INSTDIR
 
    # create the uninstaller
    WriteUninstaller "$INSTDIR\uninstall.exe"

    # Add your files with "File (Name of the file)", example: "File "$DESKTOP\MyApp.exe"" ($DESKTOP is Desktop folder); or add your folders always with "File (Name of the folder)\*", always add your folders with an asterisk, example: "File /r $DESKTOP\MyApp\*" (this will add its files and (with /r its subfolders)).
    File /r "C:\Users\kmedr\OneDrive\Desktop\backends-projects\Utils\barebones\builds\latest\*"
    File "C:\Users\kmedr\OneDrive\Desktop\backends-projects\Utils\barebones\license.txt"
    File "C:\Users\kmedr\OneDrive\Desktop\backends-projects\Utils\barebones\README.md"

    # create registry keys
    WriteRegStr HKCR "Directory\Background\shell\BareBones" "" "Generate Project Bones"
    WriteRegStr HKCR "Directory\Background\shell\BareBones\command" "" '"$INSTDIR\barebones-win.exe" "%V"'

SectionEnd
 
# uninstaller section start
Section "uninstall"
 
    # first, delete the uninstaller
    Delete "$INSTDIR\uninstall.exe"
    Delete $INSTDIR\project_bones\*
    RMDir /r $INSTDIR\project_bones
    RMDir /r $INSTDIR

# uninstaller section end
SectionEnd