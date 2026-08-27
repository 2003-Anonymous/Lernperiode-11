using System.Collections.Generic;

namespace VokabelTrainer.Models;

public static class WordList
{
    public static List<Word> Words { get; } =
    [
        new Word("Apfel", "jablko"),
        new Word("Brot",  "chleb"),
        new Word("Katze", "kot"),
        new Word("Tiger", "tygrys"),
        new Word("Biber", "bober"),
    ];
}
