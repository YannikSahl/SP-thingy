## Front-End Festpunktfelddatenbank für die DB Netz AG
Ein Projekt in Kooperation mit der DB Netz AG im Rahmen des Software Entwicklungsprojektes der HTW Berlin für das SoSe 2020.


### 1. Voraussetzungen
Die nächsten Unterkapitel beschreiben, welche Technologien / Pakete / Bibliotheken von in den einzelnen Bibliotheken benötigt werden.
Das gesamte Projekt basiert auf [.NET Core Version 3.1](https://dotnet.microsoft.com/download/dotnet-core).

#### 1.1 GUI
Die GUI beansprucht keine externen Frameworks oder NuGet Pakete.
Im Ordner `GUI/gui_resources` befinden sich Resourcen, wie Icons zur Visualisierung.

#### 1.2 DBHandler

Damit der DBHandler seine Arbeit erledigen kann, werden folgende Technologien benötigt:
*  [Microsoft Access Runtime](https://www.microsoft.com/en-us/download/confirmation.aspx?id=13255) zur Kommunikation mit der Datenbank-Datei
*  [NuGet-Paket "OleDB"](https://www.nuget.org/packages/System.Data.OleDb/4.7.1?_src=template) als weiteres Abstraktionslayer zur vereinfachten Arbeit mit der Datenbank
*  Access / Office als 32-bit Anwendung (nicht kompatibel mit 64-bit Variante!)



#### 1.3 SPHandler

* [TTCUE.NetCore.SharepointOnline.CSOM](https://www.nuget.org/packages/TTCUE.NetCore.SharepointOnline.CSOM.16.1.8029.1200) Ersatzpaket für .NetCore 
* [SharePoint Online](https://htwberlinde.sharepoint.com/sites/SWE/) mit Lizenz zur Benutzung der API

#### 1.4 SPHandlerTest
* [Microsoft.SharePointOnline.CSOM](https://www.nuget.org/packages/Microsoft.SharePointOnline.CSOM/) ab Version 16.1.20211.12000 benutzbar für .Net Standard
* [SharePoint Online](https://htwberlinde.sharepoint.com/sites/SWE/) mit Lizenz zur Benutzung der API



### 2. Dokumentation
Beim Start der Applikation öffnet sich ein Authentifizierungs Formular. Durch Anmeldung oder durch den Offline Modus, wird man ins Hauptformular weitergeleitet
Um über weiteres navigieren innerhalb der GUI zu erfahren, lesen Sie bitte die Dokumentation zur GUI (2.1)
#### 2.1 GUI
Im Hauptformular sind folgende optionen zum Navigieren durch die einzelnen Formulare möglich.
Im oberen Menü gibt es bis jetzt folgende, bis jetzt funktionierende Reiter: 
- Datei/Neue Abfrage
Wenn hier rauf geklickt wird, öffnet sich das Abfrageformular
- Extras/Einstellungen
Wenn hier rauf geklickt wird, öffnet sich das Einstellungen-Formular
- Datei/Schließen
Wenn hier rauf geklickt wird, schließen sich alle Fenster und die Applikation wird beendet

#### 2.2 DBHandler
Die Datenbankdatei "Datenmodell.accdb" ist zurzeit noch im Repository enthalten in dem Projekt DBHandler. 
Somit ist sichergestellt, dass die DBHandler-Komponente unkompliziert getestet werden kann, ohne SharePoint-Verbindung oder die Angabe eines lokalen Dateipfades.  

Das Projekt DBHandlerTest erstellt ein DBConnection Objekt, und lädt die in der "Datenmodell.accdb"-Datei vorgefundenen Daten in eine *DataTable*.  
Diese *DataTable* wird manuell manipuliert, indem eine Zeile hinzugefügt wird, und anschließend wird versucht, die "Datenmodell.accdb"-Datei mit der *DataTable* zu synchronisieren (so wie ein späterer Ablauf im Rahmen der GUI-Verwendung aussehen würde).  
Gegebenfalls müssen die Schlüsselwerte geändert werden (DBHandlerTest.Program Zeile 26 newRow["PAD"] = ...), wenn mehrere neue Zeilen hinzugefügt werden sollen, da die Einträge sonst abgelehnt werden.

#### 2.3  SharePoint Handling
Auf Grund von fehlender Infrastruktur konnte die Funktionalität zur Einbindung eines SharePoints nicht vollständig umgesetzt werden. Die Überprüfung der Verbindung zu einem SharePoint Account ist aber schon in Verwendung und Voraussetzung für die Manipulation der Datenbank.

##### 2.3.1 SPHandler
Der SPHandler beinhaltet die Funktionalität zur Überprüfung der Verbindung und wird im aktuellen Programm verwendet. Diese Bibliothek läuft nicht mit der aktuellen CSOM, sondern mit TTCUE, und verwendet deshalb die Authentifizierung über Credentials. 

##### 2.3.2 SPHandlerTest
Diese Bibliothek wurde als Test für die neu erschienene Version der CSOM erstellt und beinhaltet den Versuch der Authentifizierung über AccessToken, diese Funktionalität wird im aktuellen Programm nicht benutzt. Für mögliche Nachfolgeprojekte ist sie eventuell aber hilfreich, da die neue CSOM sehr der GraphAPI von Microsoft ähnelt. 
>In dieser Variante muss eine App Registierung in Azure vorgenommen werden.