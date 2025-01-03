# Immersive Reading Tandem

## Overview
This project is designed to enhance the reading experience by leveraging immersive technologies. It includes features like interactive text, visualization, hand-tracking and other advanced functionalities to provide a rich reading environment.

## Tech Stack
- C#
- Unity
- OpenXR
- Python

## Video presentation (in german)
You can watch a video of me using the App an a MetaQuest 3 using this [Link](https://1drv.ms/v/s!AoWzd_II_XBfjFBEKbPf8xNEaagc?e=Ecnc1v)

## Setup Instructions
To run the project locally, follow these steps: 
- German Tutorial only
- seperate ASR4DVV AI and Google Cloud Api Key needed
- ASR4DVV AI is currently not publicly available

***************************************************************************
Setup der ASR4DVV API (Funktioniert nur auf Linux):
***************************************************************************

- Unzip ASR4DVV-main.zip
- Im terminal in den Ordner gehen und im root folder die dependecies mit dem nachfolgendem Befehl installieren
	pip install -r lesetandem-requirements.txt
- Server starten mit: gunicorn --bind 0.0.0.0:8000 -w 4 "speechmeasurementapi:create_app()" --timeout 6000
- Server IP mit ifconfig im terminal rausfinden, benötigt für die Eingabe in der VR-Anwendung
	!Server und VR-Brille müssen sich in dem selben Netzwerk befinden!

***************************************************************************
Installation der APK auf der VR-Brille (Tutorial für Meta Quest 3 + Windows):
***************************************************************************

- Meta Quest Developer Hub (MQDH) herunterladen: https://developers.meta.com/horizon/downloads/package/oculus-developer-hub-win/
- MQDH Setup: https://developers.meta.com/horizon/documentation/unity/ts-odh-device-setup
- Drag and Drop der APK auf die Brille zur installation: https://developers.meta.com/horizon/documentation/unity/ts-odh-deploy-build/
- Die App kann auf der Brille unter Bibliothek -> unkown sources gefunden und gestartet werden (Name: ImmersiveReadingTandem)
- In den Einstellungen in der App die richtige Route zu der ASR4DVV API eingeben

***************************************************************************
Setup der Entwicklungsumgebung in Unity (Windows):
***************************************************************************

- Unity Editor V.2022.3.38f1 LTS über den Unity Editor installieren
- Neues Universal 3D (Universal Rendering Pipeline) Projekt in Unity Hub erstellen
- Unzip ImmersiveReadingTandem-main.zip
- Alle Dateien in das Unity Projekt kopieren, wenn aufgefordert Dateien überschreiben
- Unity Editor neu starten und im Popup "Safe-Mode" anklicken
- Compiler Errors duch installation der benötigten Packages oder Löschen von duplizierten Readmes beheben
-> Nachdem alle Compiler Errors behoben sind wechselt Unity automatisch zum normalen Modus und die App kann gestartet werden
