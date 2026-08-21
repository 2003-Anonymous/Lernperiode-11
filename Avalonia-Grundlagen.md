# Avalonia-Grundlagen für deinen Vokabeltrainer

Diese Anleitung erklärt, wie du deine App gestaltest: Buttons, Seiten wechseln, und
wie du Buttons mit C#-Code verbindest. Am Ende steht der komplette Bauplan für den
Vokabeltrainer.

Alle Code-Beispiele hier sind gegen dein Projekt kompiliert (Avalonia 12.1.1,
.NET 10, CommunityToolkit.Mvvm 8.4.2) und funktionieren.

---

## 1. Das Grundprinzip: MVVM

Avalonia arbeitet mit dem **MVVM**-Muster. Das klingt kompliziert, ist aber nur eine
Aufteilung in drei Sorten von Dateien:

| Teil | Dateiendung | Zuständig für | Ordner bei dir |
|---|---|---|---|
| **View** | `.axaml` | Das **Aussehen** (Buttons, Text, Layout) | `Views/` |
| **ViewModel** | `.cs` | Die **Logik** (Was passiert beim Klick?) | `ViewModels/` |
| **Model** | `.cs` | Die **Daten** (eine Vokabel, eine Wortliste) | `Models/` |

Die wichtigste Regel:

> Die View enthält **keine** Logik. Das ViewModel kennt **keine** Buttons.
> Verbunden werden die beiden über **Bindings**.

Warum das gut ist: Du kannst das Aussehen komplett umbauen, ohne die Logik anzufassen.

### Wie finden View und ViewModel zusammen?

Über den **DataContext**. Das ist das Objekt, auf das sich alle Bindings in einer
View beziehen. In deiner `App.axaml.cs` passiert das beim Start:

```csharp
desktop.MainWindow = new MainWindow
{
    DataContext = new MainViewModel(),   // <- hier wird verbunden
};
```

Ab jetzt gilt: Jedes `{Binding Irgendwas}` in `MainWindow.axaml` sucht `Irgendwas`
im `MainViewModel`.

---

## 2. Bindings — die Brücke zwischen View und Code

Ein Binding heisst: *"Zeige hier den Wert dieser Eigenschaft an."*

```xml
<TextBlock Text="{Binding Greeting}" />
```

Das zeigt den Inhalt der Property `Greeting` aus deinem ViewModel an.

### x:DataType — dein bester Freund

Oben in jeder View steht:

```xml
x:DataType="vm:MainViewModel"
```

Das sagt Avalonia: *"Die Bindings in dieser Datei beziehen sich auf `MainViewModel`."*

**Das ist wichtiger, als es aussieht.** In Avalonia 12 sind sogenannte *Compiled
Bindings* standardmässig aktiv. Zusammen mit `x:DataType` werden deine Bindings
beim Kompilieren geprüft. Ein Tippfehler ist damit ein **Build-Fehler**, kein
stiller Fehler zur Laufzeit:

```
Avalonia error AVLN2000: Unable to resolve property or method of name
'WoerterTYPO' on type 'VokabelTrainer.ViewModels.MainViewModel'.
```

Das ist ein Geschenk — nutze es. Wenn du `x:DataType` weglässt, verlierst du diese
Prüfung und siehst nur eine leere Anzeige, wenn du dich vertippt hast.

### Damit sich die Anzeige aktualisiert: `[ObservableProperty]`

Ein normales `public string Frage { get; set; }` reicht **nicht**. Die View muss
benachrichtigt werden, wenn sich der Wert ändert. Dafür gibt es das Attribut:

```csharp
public partial class LernViewModel : ViewModelBase   // Klasse muss "partial" sein!
{
    [ObservableProperty]
    public partial string Frage { get; set; } = "";  // Property auch "partial"!
}
```

Der Quellcode-Generator von CommunityToolkit.Mvvm baut daraus automatisch den Code,
der die View benachrichtigt. Du schreibst einfach `Frage = "Apfel";` und die
Anzeige aktualisiert sich von selbst.

> **Merke:** Sowohl die Klasse als auch die Property brauchen `partial`.
> Fehlt eines davon, gibt es einen Compiler-Fehler.

---

## 3. Buttons: zwei Wege, sie mit Code zu verbinden

### Weg A: `Click`-Event (einfach, aber nicht MVVM)

```xml
<Button Content="Klick mich" Click="MeinButton_Click" />
```

```csharp
// in MainWindow.axaml.cs
private void MeinButton_Click(object? sender, RoutedEventArgs e)
{
    // Code hier
}
```

Das funktioniert und ist für einen schnellen Test okay. **Nachteil:** Die Logik
landet in der View. Für den Vokabeltrainer nehmen wir Weg B.

### Weg B: `Command` (der MVVM-Weg — nimm diesen)

Im ViewModel schreibst du eine ganz normale Methode mit `[RelayCommand]`:

```csharp
using CommunityToolkit.Mvvm.Input;

public partial class StartViewModel : ViewModelBase
{
    [RelayCommand]
    private void Starten()
    {
        // Code hier
    }
}
```

In der View bindest du daran:

```xml
<Button Content="Lernen starten" Command="{Binding StartenCommand}" />
```

**Die Namensregel:** Aus der Methode `Starten()` wird automatisch die Property
`StartenCommand`. Also: **Methodenname + `Command`**. Wenn deine Methode
`AntwortAufdecken()` heisst, bindest du an `AntwortAufdeckenCommand`.

Das ist die häufigste Fehlerquelle am Anfang — vergisst du das `Command`-Suffix,
meckert der Compiler dank `x:DataType` aber sofort.

---

## 4. Layout: Wie du Dinge anordnest

Avalonia hat keine Pixel-Koordinaten. Du schachtelst stattdessen **Container**:

```xml
<!-- Untereinander -->
<StackPanel Spacing="10" Margin="20">
    <TextBlock Text="Oben" />
    <Button Content="Unten" />
</StackPanel>

<!-- Nebeneinander -->
<StackPanel Orientation="Horizontal" Spacing="10">
    <Button Content="Links" />
    <Button Content="Rechts" />
</StackPanel>
```

Die wichtigsten Container:

| Container | Wofür |
|---|---|
| `StackPanel` | Elemente unter- oder nebeneinander |
| `Grid` | Tabelle mit Zeilen und Spalten |
| `DockPanel` | Etwas oben/unten/links/rechts andocken |
| `Border` | Rahmen, Hintergrundfarbe, abgerundete Ecken |

Nützliche Eigenschaften auf fast jedem Element:

- `Margin="20"` — Abstand nach aussen
- `Padding="20,10"` — Abstand nach innen (waagrecht, senkrecht)
- `Spacing="10"` — Abstand **zwischen** Kindern (nur bei `StackPanel`)
- `HorizontalAlignment="Center"` / `VerticalAlignment="Center"` — Ausrichtung
- `FontSize="32"`, `Foreground="Gray"`, `Background="Transparent"`

### Etwas ein- und ausblenden

```xml
<TextBlock Text="{Binding Antwort}" IsVisible="{Binding AntwortSichtbar}" />
```

`IsVisible` bindet an ein `bool` im ViewModel. Genau so decken wir gleich die
Übersetzung auf.

---

## 5. Seiten wechseln (Navigation)

**Wichtig zu wissen:** Avalonia hat *keine* eingebaute Seiten-Navigation. Der
übliche Trick ist so einfach wie clever:

> Es gibt **ein** Fenster. Darin steckt ein `ContentControl`, das immer das
> *aktuelle ViewModel* anzeigt. "Seite wechseln" heisst: **ViewModel austauschen.**

Das Bindeglied ist der `ViewLocator.cs`, den du schon im Projekt hast. Er ist in
`App.axaml` registriert und macht genau eine Sache: Er nimmt ein ViewModel und
sucht die passende View, indem er im Namen `ViewModel` durch `View` ersetzt.

```
VokabelTrainer.ViewModels.StartViewModel   ->   VokabelTrainer.Views.StartView
VokabelTrainer.ViewModels.LernViewModel    ->   VokabelTrainer.Views.LernView
```

**Deshalb müssen die Namen exakt zusammenpassen.** Heisst dein ViewModel
`LernViewModel`, muss die View `LernView` heissen — sonst zeigt die App nur
`Not Found: ...` an.

### Der Aufbau

**MainWindow.axaml** wird zum leeren Rahmen:

```xml
<Window ...
        x:DataType="vm:MainViewModel">

    <ContentControl Content="{Binding CurrentPage}" />
</Window>
```

**MainViewModel.cs** verwaltet, welche Seite gerade dran ist:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace VokabelTrainer.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel()
    {
        CurrentPage = new StartViewModel(this);   // Startseite
    }

    public void ZeigeStart()  => CurrentPage = new StartViewModel(this);
    public void ZeigeLernen() => CurrentPage = new LernViewModel(this);
}
```

Beachte den Typ: `ViewModelBase`. Dadurch kann `CurrentPage` *jedes* deiner
ViewModels aufnehmen.

### Wie kommt eine Unterseite zurück zum MainViewModel?

Über den Konstruktor. Das `this` oben übergibt das `MainViewModel` an die
Unterseite, die es sich merkt:

```csharp
public partial class StartViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    public StartViewModel(MainViewModel main)
    {
        _main = main;
    }

    [RelayCommand]
    private void Starten()
    {
        _main.ZeigeLernen();   // <- Seitenwechsel!
    }
}
```

Das ist der einfachste funktionierende Weg. (Profis nehmen dafür einen
"NavigationService", aber für dein Projekt ist das unnötiger Ballast.)

### Und ein echtes neues Fenster?

Falls du doch mal ein separates Fenster brauchst (z.B. für Einstellungen):

```csharp
var fenster = new EinstellungenWindow
{
    DataContext = new EinstellungenViewModel()
};
fenster.Show();                                  // normales Fenster
// await fenster.ShowDialog(besitzerFenster);    // blockierendes Dialogfenster
```

Für den Vokabeltrainer brauchst du das aber nicht — der `ContentControl`-Weg ist
angenehmer.

---

## 6. Der Bauplan für deinen Vokabeltrainer

Jetzt setzen wir alles zusammen. Zielablauf:

```
Startseite  --[Button "Lernen starten"]-->  Lernseite
                                                |
                                    Wort anklicken -> Übersetzung erscheint
                                                |
                                     [Button "Weiter"] -> nächstes Wort
                                                |
                            (später) [Gewusst] / [Nicht gewusst]
```

### Schritt 1: Das Model

Neue Datei `Models/Vokabel.cs`:

```csharp
namespace VokabelTrainer.Models;

public class Vokabel
{
    public string Deutsch { get; set; } = "";
    public string Fremdsprache { get; set; } = "";

    public Vokabel(string deutsch, string fremdsprache)
    {
        Deutsch = deutsch;
        Fremdsprache = fremdsprache;
    }
}
```

Das ist schon ein deutlicher Fortschritt gegenüber deiner jetzigen Liste aus
`"Apfel: jablko"`-Strings: Du kommst jetzt getrennt an beide Hälften heran, statt
den String zerschneiden zu müssen.

### Schritt 2: Die Startseite

`ViewModels/StartViewModel.cs` — siehe oben in Abschnitt 5.

`Views/StartView.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:VokabelTrainer.ViewModels"
             x:Class="VokabelTrainer.Views.StartView"
             x:DataType="vm:StartViewModel">

    <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center" Spacing="20">
        <TextBlock Text="Vokabeltrainer" FontSize="32" HorizontalAlignment="Center" />
        <Button Content="Lernen starten"
                Command="{Binding StartenCommand}"
                HorizontalAlignment="Center"
                Padding="20,10" />
    </StackPanel>
</UserControl>
```

> **Tipp:** Lege neue Views in Visual Studio / Rider über die Vorlage
> *"Avalonia UserControl"* an. Dann bekommst du die `.axaml.cs` mit
> `InitializeComponent()` automatisch dazu. Sie sieht immer gleich aus:
>
> ```csharp
> public partial class StartView : UserControl
> {
>     public StartView() { InitializeComponent(); }
> }
> ```

### Schritt 3: Die Lernseite

`ViewModels/LernViewModel.cs` — das Herzstück:

```csharp
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VokabelTrainer.Models;

namespace VokabelTrainer.ViewModels;

public partial class LernViewModel : ViewModelBase
{
    private readonly MainViewModel _main;

    private readonly List<Vokabel> _vokabeln =
    [
        new Vokabel("Apfel", "jablko"),
        new Vokabel("Brot",  "chleb"),
        new Vokabel("Katze", "kot"),
        new Vokabel("Tiger", "tygrys"),
        new Vokabel("Biber", "bober"),
    ];

    private int _index;

    [ObservableProperty]
    public partial string Frage { get; set; } = "";

    [ObservableProperty]
    public partial string Antwort { get; set; } = "";

    [ObservableProperty]
    public partial bool AntwortSichtbar { get; set; }

    // Für später:
    public List<Vokabel> Gewusste { get; } = [];
    public List<Vokabel> NichtGewusste { get; } = [];

    public LernViewModel(MainViewModel main)
    {
        _main = main;
        ZeigeAktuelleVokabel();
    }

    private void ZeigeAktuelleVokabel()
    {
        if (_index >= _vokabeln.Count)
        {
            _main.ZeigeStart();   // Liste durch -> zurück zur Startseite
            return;
        }

        Frage = _vokabeln[_index].Deutsch;
        Antwort = _vokabeln[_index].Fremdsprache;
        AntwortSichtbar = false;   // Übersetzung wieder verstecken
    }

    [RelayCommand]
    private void AntwortAufdecken() => AntwortSichtbar = true;

    [RelayCommand]
    private void Weiter()
    {
        _index++;
        ZeigeAktuelleVokabel();
    }

    [RelayCommand]
    private void Gewusst()
    {
        Gewusste.Add(_vokabeln[_index]);
        Weiter();
    }

    [RelayCommand]
    private void NichtGewusst()
    {
        NichtGewusste.Add(_vokabeln[_index]);
        Weiter();
    }
}
```

`Views/LernView.axaml`:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:VokabelTrainer.ViewModels"
             x:Class="VokabelTrainer.Views.LernView"
             x:DataType="vm:LernViewModel">

    <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center" Spacing="20">

        <!-- Das ganze Wort ist ein Button -> anklicken deckt die Antwort auf -->
        <Button Command="{Binding AntwortAufdeckenCommand}"
                Background="Transparent"
                HorizontalAlignment="Center">
            <StackPanel Spacing="10">
                <TextBlock Text="{Binding Frage}"
                           FontSize="36"
                           HorizontalAlignment="Center" />
                <TextBlock Text="{Binding Antwort}"
                           FontSize="28"
                           Foreground="Gray"
                           HorizontalAlignment="Center"
                           IsVisible="{Binding AntwortSichtbar}" />
            </StackPanel>
        </Button>

        <StackPanel Orientation="Horizontal" Spacing="10" HorizontalAlignment="Center">
            <Button Content="Nicht gewusst" Command="{Binding NichtGewusstCommand}" />
            <Button Content="Gewusst"       Command="{Binding GewusstCommand}" />
        </StackPanel>

        <Button Content="Weiter"
                Command="{Binding WeiterCommand}"
                HorizontalAlignment="Center" />
    </StackPanel>
</UserControl>
```

Der Trick beim Aufdecken: Ein `Button` kann **beliebigen Inhalt** haben, nicht nur
Text. Hier steckt ein ganzes `StackPanel` darin. Mit `Background="Transparent"`
sieht er nicht mehr wie ein Button aus, ist aber weiterhin klickbar.

### Schritt 4: MainWindow leeren

Ersetze den Inhalt von `MainWindow.axaml` durch:

```xml
<ContentControl Content="{Binding CurrentPage}" />
```

Deine `ListBox` mit den Wörtern fliegt damit raus — die Wörter zeigt ab jetzt die
Lernseite einzeln an.

---

## 7. Ideen für danach

- **Reihenfolge mischen:** `Random.Shared.Shuffle(...)` auf einem Array, bevor es
  losgeht.
- **Fortschritt anzeigen:** eine Property
  `Fortschritt => $"{_index + 1} / {_vokabeln.Count}"`. Damit sie sich aktualisiert,
  brauchst du beim Ändern von `_index` ein `OnPropertyChanged(nameof(Fortschritt));`.
- **Ergebnisseite:** statt `_main.ZeigeStart()` am Ende ein `ErgebnisViewModel`
  anzeigen, das `Gewusste.Count` und `NichtGewusste.Count` auswertet.
- **Zweite Runde:** die `NichtGewusste`-Liste nochmal durchgehen.
- **Vokabeln aus Datei laden:** eine `.csv` einlesen statt der fest verdrahteten Liste.
- **Richtung umdrehen:** Fremdsprache -> Deutsch abfragen.

---

## 8. Häufige Stolperfallen

| Problem | Ursache |
|---|---|
| `Not Found: VokabelTrainer.Views.XyzView` | View-Name passt nicht zum ViewModel-Namen |
| Anzeige aktualisiert sich nicht | `[ObservableProperty]` vergessen, oder `partial` fehlt |
| Button macht nichts | Suffix `Command` beim Binding vergessen |
| `AVLN2000` beim Build | Tippfehler im Binding — genau lesen, der Fehler nennt den Namen |
| Liste zeigt neue Einträge nicht | `List<T>` statt `ObservableCollection<T>` verwendet |
| Bindings werden nicht geprüft | `x:DataType` in der View vergessen |

**`List<T>` oder `ObservableCollection<T>`?** Nur wenn du eine Liste an die View
bindest *und* zur Laufzeit Einträge hinzufügst oder entfernst, brauchst du
`ObservableCollection<T>`. Für `Gewusste`/`NichtGewusste` reicht `List<T>`, weil
du sie (noch) nicht anzeigst.

---

## 9. Debugging-Tipp

Dein Projekt hat `AvaloniaUI.DiagnosticsSupport` schon drin. Drücke im laufenden
Debug-Build **F12** — es öffnen sich die DevTools. Dort siehst du den
Element-Baum, alle Properties und, besonders nützlich, fehlgeschlagene Bindings.

---

## Kurzreferenz

```csharp
[ObservableProperty]                       // Property, die die View benachrichtigt
public partial string Name { get; set; }

[RelayCommand]                             // -> bindbar als {Binding MachWasCommand}
private void MachWas() { }
```

```xml
<Button Content="Text" Command="{Binding MachWasCommand}" />
<TextBlock Text="{Binding Name}" />
<TextBlock IsVisible="{Binding EinBool}" />
<ContentControl Content="{Binding CurrentPage}" />     <!-- Seitenwechsel -->
<ItemsControl ItemsSource="{Binding EineListe}" />     <!-- Liste anzeigen -->
```
