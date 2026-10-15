using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// One face piece: a pool of clippings plus how much it can jitter (random nudge/tilt/scale)
[System.Serializable]
public class FaceSlot
{
    public List<Sprite> options = new List<Sprite>();
    public Vector2 maxOffset;
    public float maxRotation;
    public Vector2 scaleRange;

    public FaceSlot(Vector2 maxOffset, float maxRotation, Vector2 scaleRange)
    {
        this.maxOffset = maxOffset;
        this.maxRotation = maxRotation;
        this.scaleRange = scaleRange;
    }
}

// A left/right pair (eyes) that shares one pool of clippings. Draw the art as LEFT eyes.
[System.Serializable]
public class FacePair : FaceSlot
{
    [Tooltip("On: both sides use the same clipping. Off: each side picks its own.")]
    public bool matchPair = true;
    [Tooltip("Flip the right side horizontally so left-eye art works for both.")]
    public bool mirrorRight = true;

    public FacePair(Vector2 maxOffset, float maxRotation, Vector2 scaleRange)
        : base(maxOffset, maxRotation, scaleRange) { }
}

// The rolled choices for one face piece. Values are relative to the slot's limits,
// so tweaking a slot's jitter on the prefab still affects existing faces.
[System.Serializable]
public class FacePart
{
    public int pick;          // which clipping; wrapped to the pool size when applied
    public Vector2 offset;    // -1..1, scaled by the slot's maxOffset
    public float rotation;    // -1..1, scaled by the slot's maxRotation
    [Range(0f, 1f)]
    public float scale;       // 0..1, mapped into the slot's scaleRange
}

// Everything needed to rebuild one character's face. Store this on the character.
[System.Serializable]
public class FaceRecipe
{
    public FacePart head, leftEye, rightEye, brows, nose, mouth, hair;
}

// Put this on the Portrait object. It finds the
// Head/LeftEye/RightEye/Brows/Nose/Mouth/Hair children by name.
// CharacterJumble.Roll() makes a new face; Apply(recipe) draws it on this portrait.
public class CharacterJumble : MonoBehaviour
{
    public FaceSlot head  = new FaceSlot(new Vector2(3f, 3f), 3f, new Vector2(0.95f, 1.05f));
    public FacePair eyes  = new FacePair(new Vector2(6f, 5f), 10f, new Vector2(0.85f, 1.15f));
    public FaceSlot brows = new FaceSlot(new Vector2(8f, 6f), 8f, new Vector2(0.9f, 1.15f));
    public FaceSlot nose  = new FaceSlot(new Vector2(8f, 6f), 8f, new Vector2(0.9f, 1.15f));
    public FaceSlot mouth = new FaceSlot(new Vector2(8f, 6f), 8f, new Vector2(0.9f, 1.15f));
    public FaceSlot hair  = new FaceSlot(new Vector2(6f, 4f), 6f, new Vector2(0.95f, 1.1f));

    // A child Image and the position you placed it at in the editor
    class Piece
    {
        public Image image;
        public Vector2 basePosition;
    }

    Dictionary<string, Piece> pieces;

    void Awake()
    {
        Bind();
    }

    void Bind()
    {
        if (pieces != null) return;
        pieces = new Dictionary<string, Piece>();
        foreach (string childName in new[] { "Head", "LeftEye", "RightEye", "Brows", "Nose", "Mouth", "Hair" })
        {
            Transform child = transform.Find(childName);
            if (child == null)
            {
                Debug.LogWarning("CharacterJumble: no child named '" + childName + "' under " + name, this);
                continue;
            }

            Image image = child.GetComponent<Image>();
            if (image == null) image = child.gameObject.AddComponent<Image>();
            image.preserveAspect = true;
            pieces[childName] = new Piece { image = image, basePosition = image.rectTransform.anchoredPosition };
        }
    }

    // Make a new random face. Doesn't need a portrait, so a Character can call it directly.
    public static FaceRecipe Roll() => Roll(new System.Random(Random.Range(int.MinValue, int.MaxValue)));

    // Same seed always gives the same face
    public static FaceRecipe Roll(int seed) => Roll(new System.Random(seed));

    static FaceRecipe Roll(System.Random rng)
    {
        return new FaceRecipe
        {
            head     = RollPart(rng),
            leftEye  = RollPart(rng),
            rightEye = RollPart(rng),
            brows    = RollPart(rng),
            nose     = RollPart(rng),
            mouth    = RollPart(rng),
            hair     = RollPart(rng),
        };
    }

    static FacePart RollPart(System.Random rng)
    {
        return new FacePart
        {
            pick = rng.Next(),
            offset = new Vector2(Lerp(rng, -1f, 1f), Lerp(rng, -1f, 1f)),
            rotation = Lerp(rng, -1f, 1f),
            scale = (float)rng.NextDouble(),
        };
    }

    // Draw a stored face on this portrait
    public void Apply(FaceRecipe recipe)
    {
        if (recipe == null) return;
        Bind();

        Place("Head", head, recipe.head, recipe.head, false);

        // matchPair: the right eye reuses the left eye's clipping but keeps its own jitter
        Place("LeftEye", eyes, recipe.leftEye, recipe.leftEye, false);
        Place("RightEye", eyes, recipe.rightEye, eyes.matchPair ? recipe.leftEye : recipe.rightEye, eyes.mirrorRight);

        Place("Brows", brows, recipe.brows, recipe.brows, false);
        Place("Nose", nose, recipe.nose, recipe.nose, false);
        Place("Mouth", mouth, recipe.mouth, recipe.mouth, false);
        Place("Hair", hair, recipe.hair, recipe.hair, false);
    }

    // Shortcut: roll and draw in one step
    public FaceRecipe Generate(int seed)
    {
        FaceRecipe recipe = Roll(seed);
        Apply(recipe);
        return recipe;
    }

    // // Re-roll with a random face; in Play mode, right-click the component in the Inspector to test
    // [ContextMenu("Randomize")]
    // public void Randomize() => Apply(Roll());

    void Place(string childName, FaceSlot slot, FacePart part, FacePart pickFrom, bool mirror)
    {
        if (part == null || pickFrom == null || slot.options.Count == 0) return;
        if (!pieces.TryGetValue(childName, out Piece piece)) return;

        piece.image.sprite = slot.options[pickFrom.pick % slot.options.Count];

        RectTransform rt = piece.image.rectTransform;
        rt.anchoredPosition = piece.basePosition + Vector2.Scale(part.offset, slot.maxOffset);
        rt.localEulerAngles = new Vector3(0, 0, part.rotation * slot.maxRotation);
        float s = Mathf.Lerp(slot.scaleRange.x, slot.scaleRange.y, part.scale);
        rt.localScale = new Vector3(mirror ? -s : s, s, 1f);
    }

    static float Lerp(System.Random rng, float a, float b) => a + (float)rng.NextDouble() * (b - a);
}
