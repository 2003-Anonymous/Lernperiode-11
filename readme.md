# Lernperiode 11

14.8 bis 11.9.2024

## Grob-Planung

Ich möchte einen Vokabeltrainer mit Avalonia programmieren, um damit Polnisch zu lernen. Man soll neue Vokabeln hinzufügen und auch schon erstellte bearbeiten und löschen können. Diese Wörter soll man dann als Karteikarten lernen können. Gelernte Vokabeln können markiert werden und diese werden beim üben nicht mehr angezeigt. Die grösste Herausforderung bei diesem Projekt ist Avalonia, da ich diese Technologie noch nie benutzt habe.
4. Wie unterscheidet sich dieses Projekt von Ihrem Projekt in 335; und wo ergänzen sich diese Projekte?

## 14.8

- [x] Als Benutzer möchte ich mit der App interagieren können, damit ich sie benutzen kann.
- [x] Als Entwickler möchte ich mich über Avalonia informieren, damit ich weiss, wie ich die App entwickeln kann.
- [x] Als Entwickler möchte ich ein Avalonia Projekt erstellen und "Hello World" ausgeben, damit ich eine lauffähige Grundlage für die weitere Entwicklung habe.

Heute habe ich ein Projekt mit Avalonia erstellt. Das hat ein wenig gedauert, da ich es in Visual Studio machen wollte und dafür zuerst VS aktualisieren musste, da die Avalonia Extension nicht da war. Weil das so lange gedauert hat, habe ich zu VS Code gewechselt. Dort war das ganze dann relativ schnell eingerichtet. Danach habe ich begonnen, dem Toutorial der Avalonia dokumentation zu folgen und ich habe einen Button mit dem Text "Hello World" darüber erstellt.

## 21.8

- [x] Als Benutzer möchte ich eine Liste mit Wörtern und ihren Bedeutungen sehen, damit ich weiss, welche Wörter ich lernen kann.
- [x] Als Benutzer möchte ich über einen Button eine neue Seite öffnen können, damit ich von der Startseite zur Wortliste gelange.
- [x] Als Benutzer möchte ich die Wörter einzeln angezeigt bekommen, damit ich mich beim Lernen auf ein Wort konzentrieren kann.
- [x] Als Benutzer möchte ich mit einem Button zum nächsten Wort wechseln können, damit ich die Liste in meinem eigenen Tempo durcharbeiten kann.

Heute habe ich eine kurze Wortliste als Beispiel erstellt, welche in der App dargestellt wird. Mit dieser Liste werden ich weiterarbeiten. Als nächstes habe ich programmiert, dass beim Starten ein neuer Viewer erstellt wird. Dort werden dann die Wörter einzeln auf Deutsch angezeigt. Wenn man das Wort anklickt, wird die Übersetzung angezeigt. Darunter kann man auswählen, ob man das Wort gewusst hat, oder nicht. Am Schluss wird noch angezeigt, wie viele Wörter man gewusst hat. Das mit den verschiedenen Viewern fand ich recht kompliziert. Vor allem, wie man diese miteinander verbindet und in welche Datei man welchen Codeteil schreibt. Ich habe dann Claude darum gebeten, mir eine Erklärung und Anleitung zu machen, wie ich das Umsetzen kann, was ich möchte. Auch trotz Anleitung war das nicht so einfach und ich habe noch nicht alles verstanden.

## 28.8

- [x] Als Benutzer möchte ich die Wortliste mit Übersetzungen vor dem Start ansehen können, damit ich die Wörter vor dem Abfragen noch einmal repetieren kann.
- [x] Als Benutzer möchte ich in dieser Ansicht Wörter hinzufügen, bearbeiten und löschen können, damit ich die Liste an meinen aktuellen Lernstoff anpassen kann.
- [x] Als Benutzer möchte ich, dass meine Wörter dauerhaft in einer Datenbank gespeichert werden, damit sie nach dem Schliessen der App noch vorhanden sind.

Heute habe ich programmiert, dass ich von der Homeseite aus die Wörterliste ansehen und auch gleich üben kann. So kann man die Wörter vor dem Abfragen nochmals lernen. Um eine bessere Darstellung von der App zu haben, habe ich noch die Grösse des Fensters auf meine Handygrösse angepasst. So kann ich mir die App besser vorstellen. Zudem habe ich hinzugefügt, dass wenn man diese Liste öffnet, man neue Wörter hinzufügen kann. Bereits vorhandene Wörter kann man bearbeiten oder auch löschen. Die Wörter werden in einer SQLite-Datenbank gespeichert.

## 4.9

- [x] Als Benutzer möchte ich gelernte Wörter als "gelernt" markieren können, damit ich beim Üben nur noch die Wörter sehe, die ich noch nicht kann.
- [x] Als Benutzer möchte ich die Wörter in zufälliger Reihenfolge abgefragt bekommen, damit ich die Übersetzungen wirklich lerne und nicht nur die Reihenfolge auswendig kann.
- [x] Als Benutzer möchte ich in der Wortliste nach einem Wort suchen können, damit ich bei vielen Vokabeln ein bestimmtes Wort schnell finde und bearbeiten kann.

Heute habe ich die Wortliste bearbeitbar gemacht: Wörter lassen sich hinzufügen, ändern und löschen. Alle Wörter werden jetzt in einer SQLite-Datenbank gespeichert, sodass sie nach dem Schliessen der App noch vorhanden sind. Ausserdem merkt sich die App, welche Wörter ich schon gewusst habe. Beim Lernen kann ich wählen, ob ich nur die offenen oder alle Wörter üben will, und ein gewusstes Wort wird wieder zu einem offenen, sobald ich es beim Lernen als "Nicht gewusst" markiere. Zum Schluss habe ich noch eingebaut, dass die Wörter in zufälliger Reihenfolge abgefragt werden und dass ich in der Wortliste nach einem Wort suchen kann. Am meisten Zeit hat mich die Datenbank gekostet, weil ich zuerst herausfinden musste, wie ich SQLite in Avalonia einbinde und wo die Datenbankdatei überhaupt gespeichert werden soll.

## 11.9

- [x] Als Benutzer möchte ich mehrere Sprachen anlegen können, damit ich in derselben App zum Beispiel Polnisch und Französisch getrennt voneinander lernen kann.
- [x] Als Benutzer möchte ich innerhalb einer Sprache eigene Sammlungen wie "Tiere" oder "Zahlen" erstellen können, damit ich gezielt ein einzelnes Thema üben kann.
- [x] Als Benutzer möchte ich eine Sammlung auswählen und nur deren Wörter abgefragt bekommen, damit ich nicht immer alle Wörter einer Sprache durchgehen muss.

Heute habe ich eingebaut, dass ich mehrere Sprachen anlegen kann. Dafür gibt es in der Datenbank eine neue Tabelle "Languages", und jedes Wort gehört über eine LanguageId zu einer Sprache. Auf der Startseite kann ich die Sprache in einem Auswahlfeld wechseln, und die Auswahl wird gespeichert, sodass beim nächsten Start wieder dieselbe Sprache aktiv ist. Über den Knopf "Sprachen verwalten" komme ich auf eine neue Seite, wo ich Sprachen hinzufügen, umbenennen und löschen kann. Die Wortliste und die Abfrage zeigen immer nur die Wörter der gewählten Sprache. Etwas knifflig war, dass meine Datenbank ja schon Wörter enthielt. Diese werden beim Start automatisch der Sprache "Polnisch" zugeordnet, damit nichts verloren geht.

Danach habe ich die Sammlungen umgesetzt. In der Datenbank gibt es dafür eine neue Tabelle "Collections", und jedes Wort kann über eine CollectionId zu einer Sammlung gehören. Weil ein Wort auch zu keiner Sammlung gehören darf, ist diese Spalte NULL erlaubt. Auf der Startseite gibt es jetzt ein zweites Auswahlfeld, in dem ich die Sammlung wähle, und über "Sammlungen verwalten" kann ich Sammlungen anlegen, umbenennen und löschen. Beim Löschen einer Sammlung bleiben die Wörter erhalten, sie verlieren nur ihre Zuordnung. In der Wortliste kann ich bei jedem Wort auswählen, zu welcher Sammlung es gehört. Als Test habe ich für Polnisch die Sammlungen Tiere, Personen und Nahrung angelegt und mit Wörtern gefüllt.

Am meisten aufgepasst habe ich beim Eintrag "Alle Wörter" in der Auswahlliste. Das ist keine echte Sammlung aus der Datenbank, sondern nur ein Platzhalter mit der Id 0. Wenn er ausgewählt ist, wird nicht gefiltert. Beim Umbenennen und Löschen muss man ihn ausschliessen, sonst würde die App versuchen, einen Datensatz zu ändern, den es gar nicht gibt.


## Fertiges Projekt
Ich habe einen Vokabeltrainer mit Avalonia programmiert. Man kann aus verschiedenen Sprachen auswählen und in der Sprache auch noch Collections erstellen, um die Wörter besser zu sortieren. Wörter können einfach in der App hinzugefügt, bearbeitet oder gelöscht werden und werden dann in einer SQLite-Datenbank gespeichert. Gewusste Wörter kann man beim lernen markieren und dann nur noch die offenen Wörter lernen. Am Ende jedes Lerndurchganges erhält man eine übersicht, wie viele Wörter man gewusst hat. Auf der Startseite ist auch ersichtlich, wie viele Wörter man schon kann.

<img width="512" height="940" alt="Lernperiode_11" src="https://github.com/user-attachments/assets/fa10c500-fbdd-4e5a-8493-e4ad5c9fe2de" />


## Reflexion
Meiner Meinung nach war es ein sehr interessantes Projekt und ich habe gelernt, wie ich mithilfe von Avalonia eine Mobile-App programmieren kann. Das Programmieren selbst hat mir aber nicht allzu grosse Freude bereitet. Obwohl es C# war, hat es sich nicht danach angefühlt, da es sehr viele neue Befehle gab und ganz anders aufgebaut war, als das, was ich bisher verwendet habe. Anfangs war die Struktur ein wenig kompliziert und ich habe nicht gleich verstanden, was jetzt in welche Datei kommt und wie diese miteinader funktionieren. Alles in allem war es eine wertvolle Erfahrung.
