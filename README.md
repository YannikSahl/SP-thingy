## Front-End Festpunktfelddatenbank für die DB Netz AG
Ein Projekt für die DB Netz AG im Rahmen des Software Entwicklungsprojektes der HTW Berlin für das SoSe 2020.


### 1. Voraussetzungen
Die nächsten Unterkapitel beschreiben, welche Technologien / Pakete / Bibliotheken von unserem Projekt benötigt werden.
Alle Projekte laufen unter [.NET Core Version 3.1](https://dotnet.microsoft.com/download/dotnet-core)

#### 1.1 Voraussetzungen: GUI
Die GUI beansprucht keine externen Frameworks oder NuGet Pakete.
Im Ordner `GUI/gui_resources` befinden sich Resourcen, wie Icons zur Visualisierung.

#### 1.2 Voraussetzungen: DBHandler
Damit der DBHandler seine Arbeit erledigen kann, werden folgende Technologien benötigt:

*  [.NET Core Version 3.1](https://dotnet.microsoft.com/download/dotnet-core) als grundlegendes Framework
*  [Microsoft Access Runtime](https://www.microsoft.com/en-us/download/confirmation.aspx?id=13255) zur Kommunikation mit der Datenbank-Datei
*  [NuGet-Paket "OleDB"](https://www.nuget.org/packages/System.Data.OleDb/4.7.1?_src=template) als weiteres Abstraktionslayer zur vereinfachten Arbeit mit der Datenbank
*  Access / Office als 32-bit Anwendung (nicht kompatibel mit 64-bit Variante!)

#### 1.3 Voraussetzungen: SPHandler

* [TTCUE.NetCore.SharepointOnline.CSOM](https://www.nuget.org/packages/TTCUE.NetCore.SharepointOnline.CSOM.16.1.8029.1200)
* [SharePoint Online](https://htwberlinde.sharepoint.com/sites/SWE/) 



### 2. Dokumentation
Beim Start der Applikation öffnet sich ein Authentifizierungs Formular. Durch Anmeldung oder durch den Offline Modus, wird man ins Hauptformular weitergeleitet
Um über weiteres navigieren innerhalb der GUI zu erfahren, lesen Sie bitte die Dokumentation zur GUI (2.1)
#### 2.1 Dokumentation: GUI
Im Hauptformular sind folgende optionen zum Navigieren durch die einzelnen Formulare möglich.
Im oberen Menü gibt es bis jetzt folgende, bis jetzt funktionierende Reiter: 
- Datei/Neue Abfrage
Wenn hier rauf geklickt wird, öffnet sich das Abfrageformular
- Extras/Einstellungen
Wenn hier rauf geklickt wird, öffnet sich das Einstellungen-Formular
- Datei/Schließen
Wenn hier rauf geklickt wird, schließen sich alle Fenster und die Applikation wird beendet

#### 2.2 Dokumentation: DBHandler
Die Datenbankdatei "Datenmodell.accdb" ist zurzeit noch im Repository enthalten in dem Projekt DBHandler. 
Somit ist sichergestellt, dass die DBHandler-Komponente unkompliziert getestet werden kann, ohne SharePoint-Verbindung oder die Angabe eines lokalen Dateipfades.  

Das Projekt DBHandlerTest erstellt ein DBConnection Objekt, und lädt die in der "Datenmodell.accdb"-Datei vorgefundenen Daten in eine *DataTable*.  
Diese *DataTable* wird manuell manipuliert, indem eine Zeile hinzugefügt wird, und anschließend wird versucht, die "Datenmodell.accdb"-Datei mit der *DataTable* zu synchronisieren (so wie ein späterer Ablauf im Rahmen der GUI-Verwendung aussehen würde).  
Gegebenfalls müssen die Schlüsselwerte geändert werden (DBHandlerTest.Program Zeile 26 newRow["PAD"] = ...), wenn mehrere neue Zeilen hinzugefügt werden sollen, da die Einträge sonst abgelehnt werden.

#### 2.3 Dokumentation: SPHandler
Wir testen das Programm im jetzigen Stand mit einem privaten SharePoint.
Für Studenten sind die WebApps von Office 365 kostenlos benutzbar, deswegen auch der Zugriff auf SharePoint.
Sie sind über ihre HTW-Mail zu dem, von uns benutzen, SharePoint hinzugefügt worden.
Wenn Sie sich also über diese Mail bei SharePoint Online anmelden, sollten die Zugriffsrechte stimmen und das Programm funktionieren.