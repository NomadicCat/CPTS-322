using System.Collections.Generic;
using System.Text;

//One thing a suspect told the detective. The lie/speculation flags are for
//the system only - never shown to the player or the other characters.
public class Testimony
{
    public string speaker;
    public string question;
    public string claim;
    public bool isLie;
    public bool isSpeculation;
}

//Shared record of what the suspects have told the detective. Characters hear
//each other's claims secondhand without learning which are lies; the system
//keeps the flags for itself (debugging, future contradiction/accusation logic).
public static class CaseFile
{
    private const int MaxClaimLength = 200;

    private static readonly List<Testimony> testimonies = new List<Testimony>();

    public static IReadOnlyList<Testimony> Testimonies => testimonies;

    public static void Reset()
    {
        testimonies.Clear();
    }

    public static void Record(string speaker, string question, string claim, bool isLie, bool isSpeculation)
    {
        testimonies.Add(new Testimony
        {
            speaker = speaker,
            question = question,
            claim = claim,
            isLie = isLie,
            isSpeculation = isSpeculation
        });
    }

    //What characters may reference: the claims only, with no hint of which
    //are lies. Returns "" when nothing has been recorded yet.
    public static string BuildCharacterSummary()
    {
        if (testimonies.Count == 0) return "";
        StringBuilder sb = new StringBuilder("What the suspects have told the detective so far (you heard this secondhand and cannot tell how much of it is true):\n");
        foreach (Testimony t in testimonies)
        {
            sb.Append("- ").Append(t.speaker).Append(" said: \"").Append(Truncate(t.claim)).Append("\"\n");
        }
        return sb.ToString();
    }

    //System view with the lie flags intact, for debugging or future mechanics.
    public static string BuildSystemReport()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Testimony t in testimonies)
        {
            string tag = t.isLie ? " [LIE]" : t.isSpeculation ? " [SPECULATION]" : " [TRUTH]";
            sb.Append(t.speaker).Append(tag).Append(" when asked \"").Append(t.question)
              .Append("\": ").Append(t.claim).Append("\n");
        }
        return sb.ToString();
    }

    private static string Truncate(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= MaxClaimLength) return text;
        return text.Substring(0, MaxClaimLength) + "...";
    }
}
