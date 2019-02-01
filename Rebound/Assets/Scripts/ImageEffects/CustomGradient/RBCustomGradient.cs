using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RBCustomGradient
{
    public enum BlendModes { Linear, Discrete };
    public enum Orientations { Horizontal, Vertical }
    public BlendModes BlendMode;
    public Orientations Orientation;
    public bool RandomizeColour;

    [System.Serializable]
    public struct ColourKey
    {
        [SerializeField]
        private Color _colour;

        [SerializeField]
        private float _time;
               
        public Color Colour
        {
            get { return _colour; }
        }

        public float Time
        {
            get { return _time; }
        }

        public ColourKey(Color colour, float time)
        {
            this._colour = colour;
            this._time = time;
        }
    }

    [SerializeField]
    private List<ColourKey> _keys = new List<ColourKey>();

    public int NumKeys { get { return _keys.Count; } }

    public RBCustomGradient()
    {
        AddKey(Color.white, 0);
        AddKey(Color.black, 1);
    }

    public int AddKey(Color colour, float time)
    {
        ColourKey newKey = new ColourKey(colour, time);

        for(int i = 0; i < NumKeys; i++)
        {
            if(newKey.Time < _keys[i].Time)
            {
                _keys.Insert(i, newKey);
                return i;
            }
        }

        _keys.Add(newKey);
        return NumKeys - 1;
    }

    public int UpdateKeyTime(int index, float time)
    {
        Color col = _keys[index].Colour;
        RemoveKey(index);
        return AddKey(col, time);
    }

    public void RemoveKey(int index)
    {
        if(NumKeys >= 2)
            _keys.RemoveAt(index);
    }

    public ColourKey GetKey(int index)
    {
        return _keys[index];
    }

    public Color Evaluate(float time)
    {
        ColourKey keyLeft = _keys[0];
        ColourKey keyRight = _keys[NumKeys - 1];

        for(int i = 0; i < NumKeys; i++)
        {
            if(_keys[i].Time <= time)
                keyLeft = _keys[i];

            if (_keys[i].Time >= time)
            {
                keyRight = _keys[i];
                break;
            }
        }

        if(BlendMode == BlendModes.Linear)
        {
            float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
            return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
        }

        return keyRight.Colour;
    }

    public void UpdateKeyColor(int index, Color col)
    {
        _keys[index] = new ColourKey(col, _keys[index].Time);
    }

    public Texture2D GetTexture(int width)
    {
        Texture2D texture = new Texture2D(width, 1);
        Color[] colours = new Color[width];

        for (int i = 0; i < width; i++)
        {
            colours[i] = Evaluate((float)i / (width - 1));
        }
        texture.SetPixels(colours);
        texture.Apply();

        if (Orientation == Orientations.Vertical)
            return rotate(texture);
        return texture;
    }

    private Texture2D rotate(Texture2D texture)
    {
        Texture2D newTexture = new Texture2D(texture.height, texture.width, texture.format, false);

        for (int i = 0; i < texture.width; i++)
            for (int j = 0; j < texture.height; j++)
                newTexture.SetPixel(j, i, texture.GetPixel(texture.width - i, j));

        newTexture.Apply();
        return newTexture;
    }
}
