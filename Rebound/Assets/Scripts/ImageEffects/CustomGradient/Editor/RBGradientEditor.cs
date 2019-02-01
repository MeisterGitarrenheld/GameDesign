using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RBGradientEditor : EditorWindow
{
    private const int _borderSize = 10;
    private const float keyWidth = 10;
    private const float keyHight = 20;

    private Rect _gradientPreviewRect;
    private RBCustomGradient _gradient;
    private Rect[] _keyRects;
    private bool _mouseIsDownOverKey;
    private int _selectedKeyIndex;
    private bool _needsRepaint;

    private void OnGUI()
    {
        Draw();
        HandleInput();

        if(_needsRepaint)
        {
            _needsRepaint = false;
            Repaint();
        }
    }

    private void Draw()
    {
        _gradientPreviewRect = new Rect(_borderSize, _borderSize, position.width - _borderSize * 2, 25);
        GUI.DrawTexture(_gradientPreviewRect, _gradient.GetTexture((int)_gradientPreviewRect.width));

        _keyRects = new Rect[_gradient.NumKeys];
        for (int i = 0; i < _gradient.NumKeys; i++)
        {
            RBCustomGradient.ColourKey key = _gradient.GetKey(i);
            Rect keyRect = new Rect(_gradientPreviewRect.x + _gradientPreviewRect.width * key.Time - keyWidth / 2f, _gradientPreviewRect.yMax + _borderSize, keyWidth, keyHight);

            if (i == _selectedKeyIndex)
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
            else
                EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.white);

            EditorGUI.DrawRect(keyRect, key.Colour);
            _keyRects[i] = keyRect;
        }

        Rect settingsRect = new Rect(_borderSize, _keyRects[0].yMax + _borderSize, position.width - _borderSize * 2, position.height);

        GUILayout.BeginArea(settingsRect);

        EditorGUI.BeginChangeCheck();
        Color newColour = EditorGUILayout.ColorField(_gradient.GetKey(_selectedKeyIndex).Colour);
        if(EditorGUI.EndChangeCheck())
        {
            _gradient.UpdateKeyColor(_selectedKeyIndex, newColour);
        }
        _gradient.BlendMode = (RBCustomGradient.BlendModes)EditorGUILayout.EnumPopup("Blend mode", _gradient.BlendMode);
        _gradient.RandomizeColour = EditorGUILayout.Toggle("Randomize color", _gradient.RandomizeColour);
        _gradient.Orientation = (RBCustomGradient.Orientations)EditorGUILayout.EnumPopup("Orientation", _gradient.Orientation);

        GUILayout.EndArea();
    }

    private void HandleInput()
    {
        Event guiEvent = Event.current;

        //Left click mouse down
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            for (int i = 0; i < _keyRects.Length; i++)
            {
                if (_keyRects[i].Contains(guiEvent.mousePosition))
                {
                    _mouseIsDownOverKey = true;
                    _selectedKeyIndex = i;
                    _needsRepaint = true;
                    break;
                }
            }

            if (!_mouseIsDownOverKey)
            {
                float keyTime = Mathf.InverseLerp(_gradientPreviewRect.x, _gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                var interpolatedColour = _gradient.Evaluate(keyTime);
                var randomColour = new Color(Random.value, Random.value, Random.value);
                _selectedKeyIndex = _gradient.AddKey(_gradient.RandomizeColour ? randomColour : interpolatedColour, keyTime);
                _mouseIsDownOverKey = true;
                _needsRepaint = true;
            }
        }

        //Left click mouse up
        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
            _mouseIsDownOverKey = false;

        //Left click drag
        if (_mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
        {
            float keyTime = Mathf.InverseLerp(_gradientPreviewRect.x, _gradientPreviewRect.xMax, guiEvent.mousePosition.x);
            _selectedKeyIndex = _gradient.UpdateKeyTime(_selectedKeyIndex, keyTime);
            _needsRepaint = true;
        }

        //Backspace or del
        if ((guiEvent.keyCode == KeyCode.Backspace || guiEvent.keyCode == KeyCode.Delete) && guiEvent.type == EventType.KeyDown)
        {
            _gradient.RemoveKey(_selectedKeyIndex);
            if (_selectedKeyIndex >= _gradient.NumKeys)
                _selectedKeyIndex--;
            _needsRepaint = true;
        }
    }

    public void SetGradient(RBCustomGradient gradient)
    {
        this._gradient = gradient;
    }

	private void OnEnable()
    {
        titleContent.text = "Gradient Editor";
        position.Set(position.x, position.y, 400, 150);
        minSize = new Vector2(200, 170);
        maxSize = new Vector2(1920, 170);
    }

    private void OnDisable()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
